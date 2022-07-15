using JetBrains.Annotations;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// A simple solving strategy to test y=x.
	/// </summary>
	[UsedImplicitly]
	internal class SecondOrderSolvingStrategyLinear
		: SecondOrderSolvingStrategyBase<SecondOrderSolvingStrategyLinear>
	{
		/// <inheritdoc />
		public override SecondOrderSolvingStrategy StrategyType => SecondOrderSolvingStrategy.Linear;

		/// <inheritdoc />
		public override void RecalculateConstants(ref SecondOrderState state, float initialValue) { }

		/// <inheritdoc />
		protected override void OnNewValue(ref SecondOrderState state, float deltaTime, float targetValue, float targetVelocity)
			=> state.CurrentValue = targetValue;
	}
}
