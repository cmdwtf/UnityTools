using System.Collections.Generic;
using System.Linq;

namespace cmdwtf.UnityTools.Filters
{
	public static class FilterExtensions
	{
		public static float SimpleAverage(this Queue<float> samples)
			=> samples.Sum(f => f) / samples.Count;

		public static float SimpleAverage(this IList<float> samples)
			=> samples.Sum(f => f) / samples.Count;
	}
}
