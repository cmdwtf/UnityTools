using System;

using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// Data common to all DynamicsTransformMutator<...>s.
	/// </summary>
	[Serializable]
	public abstract class DynamicsTransformMutator
	{
		public bool enabled;
		public DynamicsUpdateMode mode;
		public Space space;
		public DeltaTimeSource deltaTimeSource;
		[SerializeReference]
		protected IDynamicsTransformComponent mutator;

		public Func<float> CustomDeltaTimeSource { get; set; }

		protected float GetDeltaTime()
			=> deltaTimeSource switch
			{
				DeltaTimeSource.Zero => 0,
				DeltaTimeSource.DeltaTime => Time.deltaTime,
				DeltaTimeSource.UnscaledDeltaTime => Time.unscaledDeltaTime,
				DeltaTimeSource.FixedUnscaledDeltaTime => Time.fixedUnscaledDeltaTime,
				DeltaTimeSource.FixedDeltaTime => Time.fixedDeltaTime,
				DeltaTimeSource.MaximumDeltaTime => Time.maximumDeltaTime,
				DeltaTimeSource.SmoothDeltaTime => Time.smoothDeltaTime,
				DeltaTimeSource.MaximumParticleDeltaTime => Time.maximumParticleDeltaTime,
				DeltaTimeSource.Custom => CustomDeltaTimeSource?.Invoke() ?? 0,
				_ => throw new ArgumentOutOfRangeException(nameof(deltaTimeSource), deltaTimeSource, null)
			};
	}

	/// <summary>
	/// A small collection of associated data that represents
	/// the ability to modify an aspect of a <see cref="Transform"/>,
	/// plus the actual second order system.
	/// </summary>
	/// <typeparam name="T">The type of modifier component.</typeparam>
	/// <typeparam name="TValue">The type of data the modifier acts on.</typeparam>
	[Serializable]
	public abstract class DynamicsTransformMutator<T, TValue>
		: DynamicsTransformMutator
		where TValue : struct
		where T : class, IDynamicsTransformComponent<TValue>, new()
	{
		public T Mutator => mutator as T;

		protected DynamicsTransformMutator()
		{
			mutator = new T();
		}

		/// <summary>
		/// Updates the destination <see cref="Transform"/> component from the
		/// source.
		/// </summary>
		/// <param name="source">The source transform to get updated data from.</param>
		/// <param name="destination">The target transform to modify with updated data.</param>
		public void Update(ref Transform source, ref Transform destination)
		{
			float time = GetDeltaTime();
			bool copyOnly = mode == DynamicsUpdateMode.CopyOnUpdate;

			TValue val = copyOnly
				? GetSourceValue(ref source)
				: Mutator.Update(time,  GetSourceValue(ref destination), GetSourceValue(ref source));

			SetDestinationValue(ref destination, val);
		}

		/// <summary>
		/// Resets the dynamics with the current state of the given source.
		/// </summary>
		/// <param name="source">The <see cref="Transform"/> to reset the dynamics on.</param>
		public void Reset(ref Transform source)
		{
			Mutator.Reset(GetSourceValue(ref source));
		}

		/// <summary>
		/// Gets the desired value (chosen by <see cref="space"/>) from the source <see cref="Transform"/>.
		/// </summary>
		/// <param name="source">The <see cref="Transform"/> to get the value from.</param>
		/// <returns>The desired value.</returns>
		protected abstract TValue GetSourceValue(ref Transform source);

		/// <summary>
		/// Sets the desired value (chosen by <see cref="space"/>) on the destination <see cref="Transform"/>.
		/// </summary>
		/// <param name="destination">The <see cref="Transform"/> to modify.</param>
		/// <param name="value">The value to set on the destination.</param>
		protected abstract void SetDestinationValue(ref Transform destination, TValue value);
	}
}
