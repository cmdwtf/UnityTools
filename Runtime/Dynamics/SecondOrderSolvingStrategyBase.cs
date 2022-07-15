using System;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// Shared functionality between all SecondOrderDynamicsSolvingStrategy classes.
	/// Uses the CRTP to automatically implement the static instance property.
	/// </summary>
	internal abstract class SecondOrderSolvingStrategyBase<TDerived>
		: ISecondOrderSolvingStrategyF where TDerived  : ISecondOrderSolvingStrategy<float>, new()
	{
		// a singleton instance; will be collected by the registry.
		// ReSharper disable once MemberCanBePrivate.Global -- reflection can't find private static inherited members.
		public static ISecondOrderSolvingStrategy Instance { get; } = new TDerived();

		static SecondOrderSolvingStrategyBase()
		{
			if (Instance.StrategyType == SecondOrderSolvingStrategy.Unknown)
			{
				throw new TypeLoadException(
					$"Instance of {Instance.GetType().FullName} created with unexpected strategy type: {Instance.StrategyType}");
			}
		}

		/// <inheritdoc />
		public virtual SecondOrderSolvingStrategy StrategyType => SecondOrderSolvingStrategy.Unknown;

		/// <inheritdoc />
		public abstract void RecalculateConstants(ref SecondOrderState state, float initialValue);

		/// <inheritdoc />
		public float UpdateStrategy(ref SecondOrderState state, float deltaTime, float targetValue, float? targetVelocity)
		{
			float targetVelocityActual = UpdateVelocity(ref state, deltaTime, targetValue, targetVelocity);
			OnNewValue(ref state, deltaTime, targetValue, targetVelocityActual);
			return state.CurrentValue;
		}


		/// <inheritdoc cref="UpdateStrategy"/>
		protected abstract void OnNewValue(ref SecondOrderState state, float deltaTime, float targetValue, float targetVelocity);

		/// <summary>
		/// A static helper to integrate a given <see cref="state"/> towards a new <see cref="targetValue"/>.
		/// The caller may optionally provide <see cref="k1Override"/> and <see cref="k2Override"/> to use
		/// instead of the pre-calculated constants in <see cref="state"/>. This allows the caller to
		/// clamp those values to safe ranges before integrating.
		/// </summary>
		/// <param name="state">The state to update.</param>
		/// <param name="deltaTime">The elapsed time since the previous update.</param>
		/// <param name="targetValue">The current target value.</param>
		/// <param name="targetVelocity">The velocity the target value is moving at.</param>
		/// <param name="k1Override">An optional override for the K1 constant.</param>
		/// <param name="k2Override">An optional override for the K2 constant.</param>
		protected static void Integrate(ref SecondOrderState state,
			float deltaTime,
			float targetValue,
			float targetVelocity,
			float? k1Override = null,
			float? k2Override = null
		)
		{
			float k1 = k1Override ?? state.K1;
			float k2 = k2Override ?? state.K2;

			// integrate position by velocity
			state.CurrentValue += (deltaTime * state.CurrentVelocity);

			// integrate velocity by acceleration
			state.CurrentVelocity += (deltaTime * (targetValue + (state.K3 * targetVelocity) - state.CurrentValue - (k1 * state.CurrentVelocity)) / k2);
		}

		private static float UpdateVelocity(ref SecondOrderState state,
			float deltaTime,
			float targetValue,
			float? targetVelocityOrNull
		)
		{
			float estimatedVelocity = (targetValue - state.PreviousTargetValue) / deltaTime;
			float xd = targetVelocityOrNull.GetValueOrDefault(estimatedVelocity);
			state.PreviousTargetValue = targetValue;
			return xd;
		}
	}
}
