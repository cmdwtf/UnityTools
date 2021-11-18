using System;
using System.Linq;

namespace cmdwtf.UnityTools.Filters
{
	public class WeightedMovingAverage : AverageBase<float>
	{
		public WeightedMovingAverage(int windowSize)
			: base(windowSize) { }

		public override float Sample(float sample)
		{
			AppendSample(sample);
			int remainingSamples = SampleCount;

			float dividend = Samples.Sum(s => s * remainingSamples--);
			float divisor = (SampleCount * (SampleCount + 1)) / 2.0f;

			Value = dividend / divisor;
			return Value;
		}
	}
}
