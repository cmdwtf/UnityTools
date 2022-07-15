using JetBrains.Annotations;

using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// Solve the second order system using the pole-zero matching (a.k.a. matched Z-transform method)
	/// This will be more accurate than the Euler methods (especially for high velocities),
	/// but be more computationally expensive.
	/// </summary>
	/// <remarks>
	/// See also: https://www.youtube.com/watch?v=KPoeNZZ6H4s
	/// See also: https://en.wikipedia.org/wiki/Matched_Z-transform_method
	/// </remarks>
	[UsedImplicitly]
	internal class SecondOrderSolvingStrategyPoleZeroMatching
		: SecondOrderSolvingStrategyBase<SecondOrderSolvingStrategyPoleZeroMatching>
	{
		/// <inheritdoc />
		public override SecondOrderSolvingStrategy StrategyType => SecondOrderSolvingStrategy.PoleZeroMatching;

		/// <inheritdoc />
		public override void RecalculateConstants(ref SecondOrderState state, float initialValue)
		{
			state.W = 2 * Mathf.PI * state.F;
			state.D = state.W * Mathf.Sqrt(Mathf.Abs((state.Z * state.Z) - 1));
			state.K1 = state.Z / (Mathf.PI * state.F);
			state.K2 = 1 / (state.W * state.W);
			state.K3 = state.R * state.Z / state.W;
			state.PreviousTargetValue = initialValue;
			state.CurrentValue = initialValue;
			state.CurrentVelocity = default;
		}

		/// <inheritdoc />
		protected override void OnNewValue(ref SecondOrderState state, float deltaTime, float targetValue, float targetVelocity)
		{
			// values to prevent jitter & instability
			// k1Stable is only calculated if usePoleZeroMatching is enabled.
			float k1Stable;
			float k2Stable;

			bool assumeK1Stable = state.W * deltaTime < state.Z;

			if (assumeK1Stable)
			{
				// clamp k2 to guarantee stability without jitter
				k1Stable = state.K1;
				k2Stable = Mathf.Max(state.K2, (deltaTime * deltaTime / 2) + (deltaTime * state.K1 / 2), deltaTime * state.K1);
			}
			else
			{
				// use pole matching when the system is very fast
				float t1 = Mathf.Exp(-state.Z * state.W * deltaTime);
				float alpha = 2 * t1 * (state.Z <= 1 ? Mathf.Cos(deltaTime * state.D) : (float)System.Math.Cosh(deltaTime * state.D));
				float beta = t1 * t1;
				float t2 = deltaTime / (1 + beta - alpha);
				k1Stable = (1 - beta) * t2;
				k2Stable = deltaTime * t2;
			}

			Integrate(ref state, deltaTime, targetValue, targetVelocity, k1Stable, k2Stable);
		}
	}
}
