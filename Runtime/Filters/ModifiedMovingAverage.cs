

using UnityEngine;

namespace cmdwtf.UnityTools.Filters
{
	public class ModifiedMovingAverage : VirtualWindowAverage
	{
		protected virtual float WeightDividend => 1f;

		public ModifiedMovingAverage(int warmupWindow, float windowSize)
			: base(warmupWindow, windowSize) { }

		public override float Sample(float sample)
		{
			if (!WarmedUp)
			{
				return base.Sample(sample);
			}

			//Value = (((Period - 1) * Value) + sample) / Period;
			float weight = WeightDividend / (VirtualWindowSize + 1);
			float nWeight = 1 - weight;
			Value = (sample * weight) + (Value * nWeight);

			return Value;
		}
	}
}
