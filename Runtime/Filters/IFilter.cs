using System.Collections.Generic;

namespace cmdwtf.UnityTools.Filters
{
	public interface IFilter<T>
	{
		/// <summary>
		/// The type of filtering applied.
		/// </summary>
		FilterType Type { get; }

		/// <summary>
		/// The name of the filter class.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// The current filtered value.
		/// </summary>
		T Value { get; }

		/// <summary>
		/// Update the filter with the given sample.
		/// </summary>
		/// <param name="sample">The sample to update with.</param>
		T Sample(T sample);

		/// <summary>
		/// Resets the filter to it's initial state, clearing samples.
		/// </summary>
		void Reset();
	}

	public interface IFilterMultiple<T> : IFilter<IEnumerable<T>>
	{
		/// <summary>
		/// Update the filter with the given samples, with more than one.
		/// </summary>
		/// <param name="sample">The sample to update with.</param>
		IEnumerable<T> Sample(params T[] sample);
	}
}
