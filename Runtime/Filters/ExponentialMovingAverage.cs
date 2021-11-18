using System;
using System.Linq;

namespace cmdwtf.UnityTools.Filters
{
	public class ExponentialMovingAverage : ModifiedMovingAverage
	{
		protected override float WeightDividend => 2f;

		public ExponentialMovingAverage(int warmupWindow, float virtualWindow)
			: base(warmupWindow, virtualWindow) { }
	}


}
