using System;

using cmdwtf.UnityTools.Collections;

namespace cmdwtf.UnityTools.Filters
{
	public abstract class AverageBase<T> : FilterBase<T>, IAverage<T>
	{
		public override FilterType Type => FilterType.Average;

		public virtual int WindowSize { get; private set; }

		public virtual int SampleCount => Samples.Count;
		protected Deque<T> Samples { get; private set; } = new Deque<T>();

		protected T LastSampleRemoved { get; private set; }
		protected bool WindowFull => Samples.Count >= WindowSize;

		public AverageBase(int windowSize)
		{
			if (windowSize < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(windowSize), "Must be greater than or equal to 0");
			}

			WindowSize = windowSize;
		}

		/// <summary>
		/// Adds a new sample to the sample collection, removing one if it exceeds the window size.
		/// </summary>
		/// <param name="sample">The sample to add.</param>
		/// <returns>The value of the sample removed, if any, otherwise <cref langword="default"/>.</returns>
		protected T AppendSample(T sample)
		{
			if (WindowSize < 1)
			{
				return default;
			}

			Samples.Enqueue(sample);

			// if we overflowed our window, remove it, and return it.
			if (Samples.Count > WindowSize)
			{
				LastSampleRemoved = Samples.Dequeue();
				return LastSampleRemoved;
			}

			return default;
		}

		protected void UndoLastSampleAppend()
		{
			Samples.RemoveFromBack();
			Samples.AddToFront(LastSampleRemoved);
		}

		public override void Reset()
		{
			Samples.Clear();
			Value = default;
		}
	}
}
