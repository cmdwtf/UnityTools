using System;

using JetBrains.Annotations;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// An interface representing a second order solving strategy.
	/// </summary>
	public interface ISecondOrderSolvingStrategy
	{
		/// <summary>
		/// A singleton instanceof the strategy.
		/// </summary>
		static ISecondOrderSolvingStrategy Instance { get; } = null;

		/// <summary>
		/// The type of strategy employed by the instance.
		/// </summary>
		SecondOrderSolvingStrategy StrategyType { get; }
	}

	/// <summary>
	/// A slightly more specific interface representing a second order solving strategy,
	/// that includes the expected methods as well as selects the type expected to be used for values.
	/// </summary>
	/// <typeparam name="TComponent"></typeparam>
	internal interface ISecondOrderSolvingStrategy<TComponent>
		: ISecondOrderSolvingStrategy where TComponent : struct
	{
		/// <summary>
		/// Have the strategy recalculate constants in the <see cref="state"/> with the given initial value.
		/// </summary>
		/// <param name="state">The state with constants that should be modified.</param>
		/// <param name="initialValue">The new initial value.</param>
		void RecalculateConstants(ref SecondOrderState state, TComponent initialValue);

		/// <summary>
		/// Updates the referenced state with the new <see cref="targetValue"/> and <see cref="deltaTime"/>.
		/// The caller may optionally provide the velocity of the <see cref="targetValue"/> in <see cref="targetVelocity"/>,
		/// but if they do not, it is up to the strategy to estimate it.
		/// </summary>
		/// <param name="state">The state to update.</param>
		/// <param name="deltaTime">The amount of time that has elapsed since the previous update.</param>
		/// <param name="targetValue">The current target value.</param>
		/// <param name="targetVelocity">The current target velocity, or null if unknown.</param>
		/// <returns>The newest current value of the state.</returns>
		TComponent UpdateStrategy(ref SecondOrderState state, TComponent deltaTime, TComponent targetValue, TComponent? targetVelocity);
	}

	/// <summary>
	/// A specialized <see cref="ISecondOrderSolvingStrategy"/> that uses the type <see langword="float"/> type.
	/// </summary>
	[UsedImplicitly]
	internal interface ISecondOrderSolvingStrategyF
		: ISecondOrderSolvingStrategy<float>
	{

	}

	/// <summary>
	/// A specialized <see cref="ISecondOrderSolvingStrategy"/> that uses the type <see langword="double"/> type.
	/// </summary>
	[UsedImplicitly]
	internal interface ISecondOrderSolvingStrategyD
		: ISecondOrderSolvingStrategy<double>
	{

	}

}
