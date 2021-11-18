using System;
using System.Linq;

namespace cmdwtf.UnityTools.Filters
{
	public class SimpleMovingAverage : AverageBase<float>
	{
		protected float Accumulator { get; private set; }

		public SimpleMovingAverage(int windowSize)
			: base(windowSize)
		{
		}

		public override float Sample(float sample)
		{
			Accumulator += sample;
			Accumulator -= AppendSample(sample);
			Value = Accumulator / SampleCount;
			return Value;
		}
	}
}
