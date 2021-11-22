using System;
using System.Collections.Generic;
using System.Linq;

namespace cmdwtf.UnityTools.Filters
{
	public sealed class FilterSequence<T> : IFilter<T>
	{
		public FilterType Type => FilterType.Custom;
		public string Name => string.Join(", ", Filters.Select(f => f.Name));
		public T Value { get; private set; }
		private Queue<IFilter<T>> Filters { get; } = new Queue<IFilter<T>>();

		private FilterSequence(params IFilter<T>[] filters)
		{
			foreach (IFilter<T> filter in filters)
			{
				AddFilter(filter);
			}
		}

		public static FilterSequence<T> With(params IFilter<T>[] filters) => new FilterSequence<T>(filters);

		public T Sample(T sample)
		{
			foreach (IFilter<T> filter in Filters)
			{
				sample = filter.Sample(sample);
			}

			Value = sample;
			return Value;
		}

		public void Reset()
		{
			foreach (IFilter<T> filter in Filters)
			{
				filter.Reset();
			}
		}

		private void AddFilter(IFilter<T> filter)
		{
			if (Filters.Contains(filter))
			{
				throw new ArgumentException("Filter already exists in the collection.", nameof(filter));
			}

			Filters.Enqueue(filter);
		}
	}
}
