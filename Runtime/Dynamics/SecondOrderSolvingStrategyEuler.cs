using JetBrains.Annotations;

using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// Solve the second order system using the Semi-implicit Euler method.
	/// </summary>
	/// <remarks>
	/// See also: https://www.youtube.com/watch?v=KPoeNZZ6H4s
	/// See also: https://en.wikipedia.org/wiki/Semi-implicit_Euler_method
	/// See also: https://en.wikipedia.org/wiki/Verlet_integration
	/// </remarks>
	[UsedImplicitly]
	internal class SecondOrderSolvingStrategyEuler
		: SecondOrderSolvingStrategyBase<SecondOrderSolvingStrategyEuler>
	{
		/// <inheritdoc />
		public override SecondOrderSolvingStrategy StrategyType => SecondOrderSolvingStrategy.SemiImplicitEulerUnstable;

		/// <inheritdoc />
		public override void RecalculateConstants(ref SecondOrderState state, float initialValue)
			=> DefaultConstantCalculation(ref state, initialValue);

		/// <inheritdoc />
		protected override void OnNewValue(ref SecondOrderState state, float deltaTime, float targetValue, float targetVelocity)
			=> Integrate(ref state, deltaTime, targetValue, targetVelocity);

		/// <summary>
		/// An internal helper to calculate constant values in the majority of Euler solvers.
		/// </summary>
		/// <param name="state">The current state to update.</param>
		/// <param name="initialValue">The new initial value.</param>
		internal static void DefaultConstantCalculation(ref SecondOrderState state, float initialValue)
		{
			state.K1 = state.Z / (Mathf.PI * state.F);

			float tauFrequency = (2 * Mathf.PI * state.F);

			state.K2 = 1 / (tauFrequency * tauFrequency);
			state.K3 = state.R / tauFrequency;

			state.MaximumTimeStep = 0.8f * (Mathf.Sqrt((4 * state.K2) + (state.K1 * state.K1)) - state.K1);

			state.PreviousTargetValue = initialValue;
			state.CurrentValue = initialValue;
			state.CurrentVelocity = default;
		}
	}
}
