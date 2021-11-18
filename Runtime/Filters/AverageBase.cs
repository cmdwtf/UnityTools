using System;
using System.Collections.Generic;

namespace cmdwtf.UnityTools.Filters
{
	public abstract class AverageBase<T> : IAverage<T>
	{
		public virtual FilterType Type => FilterType.Average;

		private string _name;
		public virtual string Name => _name ??= GetType().Name;

		public virtual int WindowSize { get; private set; }
		public T Value { get; protected set; }

		public virtual int SampleCount => Samples.Count;
		protected Queue<T> Samples { get; private set; } = new Queue<T>();

		public AverageBase(int windowSize)
		{
			if (windowSize < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(windowSize), "Must be greater than or equal to 0");
			}

			WindowSize = windowSize;
		}

		protected T AppendSample(T sample)
		{
			if (WindowSize < 1)
			{
				return default;
			}

			Samples.Enqueue(sample);

			if (Samples.Count > WindowSize)
			{
				return Samples.Dequeue();
			}

			return default;
		}

		public virtual void Reset()
		{
			Samples.Clear();
			Value = default;
		}

		public abstract T Sample(T sample);
	}
}
