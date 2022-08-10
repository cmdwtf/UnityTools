using System;

using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// User-adjustable settings for a second-order dynamics system.
	/// #todo: expose to external assemblies
	/// </summary>
	[Serializable]
	internal struct SecondOrderSettings
	{
		#region Private Constants

		internal const float DefaultFMinimum = 0f;
		internal const float DefaultFMaximum = 20f;
		internal const float DefaultZMinimum = 0f;
		internal const float DefaultZMaximum = 2f;
		internal const float DefaultRMinimum = -5f;
		internal const float DefaultRMaximum = 5f;
		internal const float DefaultDeltaTimeRangeMinimum = 0f;
		internal const float DefaultDeltaTimeRangeMaximum = 5f;

		internal const float DampingUndamped = 0;
		internal const float DampingCritical = 1;
		internal const float DampingSmooth = DampingCritical;

		internal const float ResponsivenessSlow = 0;
		internal const float ResponsivenessNatural = 1;
		internal const float ResponsivenessAnticipate = -2;
		internal const float ResponsivenessOvershoot = 2;

		internal const float DeltaTimeRangeRealtime = 1f;

		internal const float MaxStableValue = 1000000f;

		#endregion

		#region Seralized Fields

		[Tooltip("f: The natural frequency of the system, in Hz. If the system isn't dampened, this is the frequency " +
		         "it will oscillate at.")]
		public ValueRange frequency;
		[Tooltip("ζ: The damping coefficient. Affects how quickly the system settles on the target value. 0 = 'undamped', " +
		         "between 0 and 1, 'underdamped'. 1 = 'critical damping' (e.g.: Unity's SmoothDamp())")]
		public ValueRange damping;
		[Tooltip("r: The initial responsiveness of the system. With a r of 0, the system will take some time to move towards the target. " +
		         "A value of 1 will cause it to react immediately. A value greater than 1 will cause overshoot, " +
		         "while a negative value will cause an undershoot before moving towards the target. " +
		         "The r of a typical mechanical system is 2.")]
		public ValueRange responsiveness;

		[Tooltip("The mathematics strategy to use to solve the system.")]
		public SecondOrderSolvingStrategy solvingStrategy;

		[Tooltip("The strategy used to sample the input values (f, ζ, and r) that generate the system constants.")]
		public SamplingMode inputSampleMode;

		[Tooltip("How the system will respond when it falls out of a stable range.")]
		public UnstableHandlingMode unstableHandlingMode;

		[Tooltip("Controls if the system should reset itself to it's last reset state and initial value, should the system fail.")]
		public bool unstableAutoReset;

		[Tooltip("A multiplier for the time step passed to the system. Values lower than 1 will cause slower movement, higher faster.")]
		public ValueRange deltaTimeScale;

		#endregion

		public SecondOrderSettings(float frequencyF, float dampingZ, float responsivenessR,
			SecondOrderSolvingStrategy solvingStrategy = SecondOrderSolvingStrategy.PoleZeroMatching,
			SamplingMode inputSampleMode = SamplingMode.Fixed,
			UnstableHandlingMode unstableHandlingMode = UnstableHandlingMode.ReturnTarget,
			bool unstableAutoReset = false
		)
		{
			// build the f/ζ/r/dtscale ranges with fixed ranges at the given values.
			frequency = new ValueRange(frequencyF, frequencyF, DefaultFMinimum, DefaultFMaximum);
			damping = new ValueRange(dampingZ, dampingZ, DefaultZMinimum, DefaultZMaximum);
			responsiveness = new ValueRange(responsivenessR, responsivenessR, DefaultRMinimum, DefaultRMaximum);
			deltaTimeScale = new ValueRange(DeltaTimeRangeRealtime, DeltaTimeRangeRealtime, DefaultDeltaTimeRangeMinimum, DefaultDeltaTimeRangeMaximum);

			// store desired modes
			this.inputSampleMode = inputSampleMode;
			this.solvingStrategy = solvingStrategy;
			this.unstableHandlingMode = unstableHandlingMode;
			this.unstableAutoReset = unstableAutoReset;
		}

		private float Sample(ref ValueRange rangeToSample, in SecondOrderState state)
		{
			// switch out of fixed mode.
			if (inputSampleMode != SamplingMode.Fixed && rangeToSample.forceFixedValue)
			{
				rangeToSample.forceFixedValue = false;
			}

			return inputSampleMode switch
			{
				SamplingMode.None => 0,
				SamplingMode.Fixed => rangeToSample.ForceFixed(),
				SamplingMode.Minimum => rangeToSample.Minimum(),
				SamplingMode.Midpoint => rangeToSample.Midpoint(),
				SamplingMode.Maximum => rangeToSample.Maximum(),
				SamplingMode.Random when !state._isDeserializing => rangeToSample.Random(),
				SamplingMode.Random => rangeToSample.Midpoint(), // using random during deserialization makes unity sad
				//SamplingMode.LerpByStepTime => rangeToSample.Sample(state.DeltaTime),
				//SamplingMode.LerpByScaledTime => rangeToSample.Sample(state.ElapsedTime / deltaTimeScale),
				_ => throw new ArgumentOutOfRangeException(),
			};
		}

		internal float SampleF(in SecondOrderState state)
			=> Sample(ref frequency, in state);

		internal float SampleZ(in SecondOrderState state)
			=> Sample(ref damping, in state);

		internal float SampleR(in SecondOrderState state)
			=> Sample(ref responsiveness, in state);

		internal float SampleDeltaTimeScale(in SecondOrderState state)
			=> Sample(ref deltaTimeScale, in state);

		internal void SetRangesToTheirLimits()
		{
			frequency.value = frequency.limits;
			damping.value = damping.limits;
			responsiveness.value = responsiveness.limits;
		}

		internal void ValidateRangeLimits()
		{
			if (!frequency.hasAssignedLimits)
			{
				frequency.limits = (DefaultFMinimum, DefaultFMaximum);
			}

			if (!damping.hasAssignedLimits)
			{
				damping.limits = (DefaultZMinimum, DefaultZMaximum);
			}

			if (!responsiveness.hasAssignedLimits)
			{
				responsiveness.limits = (DefaultRMinimum, DefaultRMaximum);
			}

			if (!deltaTimeScale.hasAssignedLimits)
			{
				deltaTimeScale.limits = (DefaultDeltaTimeRangeMinimum, DefaultDeltaTimeRangeMaximum);
			}
		}

		internal StabilityState GetStability(ref SecondOrderState state)
		{
			if (Mathf.Approximately(state.CurrentValue, state.TargetValue))
			{
				return StabilityState.Stable;
			}

			// if the value isn't finite or zero, we've fallen into a failure.
			if (!state.CurrentValue.IsFiniteOrZero())
			{
				return StabilityState.DenormalizedValue;
			}

			// throw out arbitrarily large numbers
			if (Mathf.Abs(state.CurrentValue) > MaxStableValue)
			{
				return StabilityState.BeyondMaximumLimit;
			}

			// edge case: a fcy of 0 means we'll never approach the target.
			if (!Mathf.Approximately(state.TargetValue, state.InitialValue) &&
			    Mathf.Approximately(state.CurrentValue, state.InitialValue) &&
			    state.CurrentVelocity == 0 &&
				state.F == 0 &&
			    state.ElapsedTime > 0)
			{
				return StabilityState.NeverApproachingTarget;
			}

			// #todo: there's probably a better way of determining if a system is unstable...

			return StabilityState.Stable;
		}
	}
}
