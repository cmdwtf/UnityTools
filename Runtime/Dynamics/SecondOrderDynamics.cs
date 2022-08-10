using System;
using System.Collections.Generic;

using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// A second-order dynamics simulation system.
	/// </summary>
	[Serializable]
	public class SecondOrderDynamics : ISimulatableDynamicsSystem, ISerializationCallbackReceiver
	{
		#region Private Constants

		private const float DefaultSaneF = 3f;
		private const float DefaultSaneZ = 1f;
		private const float DefaultSaneR = 0f;
		private const float DefaultSaneInitialValue = 0f;

		#endregion

		#region Serialized Fields

		[SerializeField]
		internal SecondOrderSettings settings;

		#endregion

		#region Private State

		private ISecondOrderSolvingStrategyF _solver;
		private SecondOrderState _state;

		#endregion

		#region Internal State Properties

		internal SecondOrderState State
		{
			get => _state;
			set => _state = value;
		}

		#endregion

		#region Public State Accessors

		/// <summary>
		/// The current frequency (f) value of the system.
		/// </summary>
		public float F => _state.F;

		/// <summary>
		/// The current damping coefficient (ζ) value of the system.
		/// </summary>
		public float Z => _state.Z;

		/// <summary>
		/// The current response (r) value of the system.
		/// </summary>
		public float R => _state.R;

		/// <summary>
		/// <see langword="true"/> if the system is currently stable.
		/// </summary>
		public bool IsStable => settings.GetStability(ref _state) == StabilityState.Stable;

		/// <summary>
		/// Gets a value representing the current stability of the system.
		/// </summary>
		public StabilityState StabilityState => settings.GetStability(ref _state);

		/// <summary>
		/// The last value the system produced.
		/// </summary>
		public float Value => _state.CurrentValue;

		/// <summary>
		/// The last stable value that the system produced.
		/// </summary>
		public float LastStableValue => _state.LastStableValue;

		#endregion

		#region Static Instance Properties

		/// <summary>
		/// A default second order dynamics system that has some sane default values to produce eased movement.
		/// </summary>
		public static SecondOrderDynamics Default => new();

		#endregion

		/// <summary>
		/// Creates a new <see cref="SecondOrderDynamics"/> with default input values.
		/// </summary>
		public SecondOrderDynamics() : this(DefaultSaneF, DefaultSaneZ, DefaultSaneR)
		{ }

		/// <summary>
		/// Creates a new <see cref="SecondOrderDynamics"/> with specified input values.
		/// </summary>
		/// <param name="f">The frequency of the system.</param>
		/// <param name="z">The dampening coefficient of the system.</param>
		/// <param name="r">The response of the system.</param>
		/// <param name="initialValue">The value the system should start from when reset.</param>
		/// <param name="strategy">The solving strategy to use for the system.</param>
		public SecondOrderDynamics(float f, float z, float r, float initialValue = DefaultSaneInitialValue, SecondOrderSolvingStrategy strategy = SecondOrderSolvingStrategy.PoleZeroMatching)
		{
			// setup initial state
			_state._isDeserializing = true;
			_state.InitialValue = initialValue;

			// store settings
			settings = new SecondOrderSettings(f, z, r, strategy);

			// calculate constants
			RecalculateConstants();

			// all finished, we are done deserializing
			_state._isDeserializing = false;
		}

		/// <summary>
		/// Resets the system to the original initial value.
		/// </summary>
		public void Reset() => ResetTemporaryIv(_state.InitialValue);

		/// <summary>
		/// Resets the system to the given initial value, and optionally
		/// the initial velocity.
		/// </summary>
		/// <param name="newIv"></param>
		/// <param name="newVelocity"></param>
		public void Reset(float newIv, float newVelocity = default)
		{
			_state.InitialValue = newIv;
			ResetTemporaryIv(_state.InitialValue, newVelocity);
		}

		/// <summary>
		/// Updates the system with the new target value and timestep.
		/// </summary>
		/// <param name="deltaTime">The amount of time elapsed since the last update.</param>
		/// <param name="targetValue">The value the system should be moving towards.</param>
		/// <param name="xdOrNull">The velocity of the target, or null if the system should estimate it.</param>
		/// <returns>The updated value from the system.</returns>
		public float Update(float deltaTime, float targetValue, float? xdOrNull = null)
		{
			if (settings.unstableAutoReset && !IsStable)
			{
				Reset();
			}

			deltaTime *= settings.SampleDeltaTimeScale(in _state);

			// update timing
			_state.DeltaTime = deltaTime;
			_state.ElapsedTime += deltaTime;

			// solve for our new step. this will update the state.
			_solver?.UpdateStrategy(ref _state, deltaTime, targetValue, xdOrNull);

			// if our system has failed, handle the failure by the desired failure mode.
			if (!IsStable)
			{
				return settings.unstableHandlingMode switch
				{
					UnstableHandlingMode.ReturnTarget => targetValue,
					UnstableHandlingMode.Return0 => 0,
					UnstableHandlingMode.ReturnLastStableValue => _state.LastStableValue,
					UnstableHandlingMode.AllowDenormalizedValues => _state.CurrentValue,
					_ => throw new NotImplementedException(),
				};
			}

			// system is still stable, record this current value as last stable.
			_state.LastStableValue = _state.CurrentValue;
			return _state.CurrentValue;
		}

		private void ValidateUserInputs()
		{
			// validate to the ranges ensure they're good
			settings.ValidateRangeLimits();
		}

		private void ResampleUserInputs()
		{
			_state.F = settings.SampleF(in _state);
			_state.Z = settings.SampleZ(in _state);
			_state.R = settings.SampleR(in _state);
			settings.SampleDeltaTimeScale(in _state);
		}

		private void RecalculateConstants()
		{
			ValidateSolver();
			ValidateUserInputs();
			ResampleUserInputs();
			settings.ValidateRangeLimits();
			_solver?.RecalculateConstants(ref _state, _state.InitialValue);
		}

		internal void ResetTemporaryIv(float iv, float newYd = default)
		{
			_state.PreviousTargetValue = iv;
			_state.CurrentValue = iv;
			_state.LastStableValue = iv;
			_state.CurrentVelocity = newYd;
			_state.DeltaTime = 0;
			_state.ElapsedTime = 0;
			RecalculateConstants();
		}

		private bool ValidateSolver()
		{
			if (_solver is not null && _solver.StrategyType == settings.solvingStrategy)
			{
				return true;
			}

			_solver = SecondOrderStrategyRegistry.Get(settings.solvingStrategy);

			if (_solver != null)
			{
				return true;
			}

			Debug.LogError($"Unable to satisfy solver for strategy: {settings.solvingStrategy}");
			return false;
		}

		internal void ApplyPreset(SecondOrderDynamicsPreset preset)
		{
			settings.frequency = preset.Frequency;
			settings.damping = preset.Damping;
			settings.responsiveness = preset.Responsiveness;
			settings.deltaTimeScale = preset.DeltaTimeScale;
			RecalculateConstants();
		}

		#region Overrides of Object

		/// <inheritdoc />
		public override string ToString()
			=> $"f={F:0.###}, " +
			   $"ζ={Z:0.###}, " +
			   $"r={R:0.###}";

		#endregion

		#region ISerializationCallbackReceiver Implementation

		void ISerializationCallbackReceiver.OnBeforeSerialize() { }

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			_state._isDeserializing = true;
			RecalculateConstants();
			_state._isDeserializing = false;
		}

		#endregion

		#region Implementation of ISimulatableDynamicsSystem

#if UNITY_EDITOR

		private Stack<SecondOrderState> _simStack = new();
		private string _serializedSettings = null;

		/// <inheritdoc />
		void ISimulatableDynamicsSystem.PushState() => _simStack.Push(_state);

		/// <inheritdoc />
		bool ISimulatableDynamicsSystem.PopState() => _simStack.TryPop(out _state);

		/// <inheritdoc />
		void ISimulatableDynamicsSystem.SetTemporarySettings(IDynamicsPreset preset)
		{
			if (preset is not SecondOrderDynamicsPreset sodp)
			{
				Debug.LogWarning($"Preset provided wasn't of type {nameof(SecondOrderDynamicsPreset)}");
				return;
			}

			if (!string.IsNullOrEmpty(_serializedSettings))
			{
				Debug.LogWarning("Attempted to set temporary settings while they were already set.");
				return;
			}

			_serializedSettings = JsonUtility.ToJson(settings);

			ApplyPreset(sodp);
		}


		/// <inheritdoc />
		void ISimulatableDynamicsSystem.ClearTemporarySettings()
		{
			if (string.IsNullOrEmpty(_serializedSettings))
			{
				Debug.LogWarning("Attempted to clear temporary settings while they were not set.");
				return;
			}

			settings = JsonUtility.FromJson<SecondOrderSettings>(_serializedSettings);
			_serializedSettings = null;

			RecalculateConstants();
		}

		/// <inheritdoc />
		float ISimulatableDynamicsSystem.UpdateSim(float deltaTime, float v) => Update(deltaTime, v, null);

		/// <inheritdoc />
		void ISimulatableDynamicsSystem.ResetTemporarySim(float iv) => ResetTemporaryIv(iv, default);

#endif // UNITY_EDITOR

		#endregion
	}
}
