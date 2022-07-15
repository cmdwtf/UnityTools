using JetBrains.Annotations;

using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// As <see cref="SecondOrderSolvingStrategyEuler"/>, but with constrained a forced
	/// maximum delta time per update; which will force multiple iterations if too large.
	/// </summary>
	/// <remarks>
	/// See also: https://www.youtube.com/watch?v=KPoeNZZ6H4s
	/// See also: https://en.wikipedia.org/wiki/Semi-implicit_Euler_method
	/// </remarks>
	[UsedImplicitly]
	internal class SecondOrderSolvingStrategyEulerStableForcedIterations
		: SecondOrderSolvingStrategyBase<SecondOrderSolvingStrategyEulerStableForcedIterations>
	{
		/// <inheritdoc />
		public override SecondOrderSolvingStrategy StrategyType => SecondOrderSolvingStrategy.SemiImplicitEulerStableForcedIterations;

		/// <inheritdoc />
		public override void RecalculateConstants(ref SecondOrderState state, float initialValue)
			=> SecondOrderSolvingStrategyEuler.DefaultConstantCalculation(ref state, initialValue);

		/// <inheritdoc />
		protected override void OnNewValue(ref SecondOrderState state, float deltaTime, float targetValue, float targetVelocity)
		{
			int iterations = (int)Mathf.Ceil(deltaTime / state.MaximumTimeStep); // take extra iterations if deltaTime > critical time

			float deltaTimePart = deltaTime / iterations; // each iteration now has a smaller step

			for (int scan = 0; scan < iterations; ++scan)
			{
				Integrate(ref state, deltaTimePart, targetValue, targetVelocity);
			}
		}
	}
}
