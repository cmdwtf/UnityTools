using JetBrains.Annotations;

using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// As <see cref="SecondOrderSolvingStrategyEuler"/>, but with constrained k2 values.
	/// </summary>
	/// <remarks>
	/// See also: https://www.youtube.com/watch?v=KPoeNZZ6H4s
	/// See also: https://en.wikipedia.org/wiki/Semi-implicit_Euler_method
	/// </remarks>
	[UsedImplicitly]
	internal class SecondOrderSolvingStrategyEulerStableClampedK2
		: SecondOrderSolvingStrategyBase<SecondOrderSolvingStrategyEulerStableClampedK2>
	{
		/// <inheritdoc />
		public override SecondOrderSolvingStrategy StrategyType => SecondOrderSolvingStrategy.SemiImplicitEulerStableClampedK2;

		/// <inheritdoc />
		public override void RecalculateConstants(ref SecondOrderState state, float initialValue)
			=> SecondOrderSolvingStrategyEuler.DefaultConstantCalculation(ref state, initialValue);

		/// <inheritdoc />
		protected override void OnNewValue(ref SecondOrderState state, float deltaTime, float targetValue, float targetVelocity)
		{
			// clamp k2 above the catastrophic error value.
			// not physically correct, but goal is to prevent failure, not produce accurate physics sim

			// clamp k2 to guarantee stability
			float k2Stable = Mathf.Max(state.K2,
				1.1f * ((deltaTime * deltaTime) / 4 + (deltaTime * state.K1 / 2)));

			Integrate(ref state, deltaTime, targetValue, targetVelocity, state.K1, k2Stable);
		}
	}
}
