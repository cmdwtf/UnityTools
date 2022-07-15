using JetBrains.Annotations;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// A simple solving strategy to test y=0.
	/// </summary>
	[UsedImplicitly]
	internal class SecondOrderSolvingStrategyNone
		: SecondOrderSolvingStrategyBase<SecondOrderSolvingStrategyNone>
	{
		/// <inheritdoc />
		public override SecondOrderSolvingStrategy StrategyType => SecondOrderSolvingStrategy.None;

		/// <inheritdoc />
		public override void RecalculateConstants(ref SecondOrderState state, float initialValue) { }

		/// <inheritdoc />
		protected override void OnNewValue(ref SecondOrderState state, float deltaTime, float targetValue, float targetVelocity)
		{
			// intentional noop
		}
	}
}
