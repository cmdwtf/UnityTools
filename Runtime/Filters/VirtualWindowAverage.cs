using System;

namespace cmdwtf.UnityTools.Filters
{
	public abstract class VirtualWindowAverage : AverageBase<float>
	{
		public bool WarmedUp => Samples.Count >= WindowSize;

		public float VirtualWindowSize { get; private set; }

		public VirtualWindowAverage(int warmupWindow, float windowSize)
			: base(warmupWindow)
		{
			VirtualWindowSize = windowSize;
		}

		public override float Sample(float sample)
		{
			if (WarmedUp)
			{
				throw new ApplicationException(
					$"{nameof(VirtualWindowAverage)}.{nameof(Sample)} shouldn't be called after warmup.");
			}

			AppendSample(sample);
			Value = Samples.SimpleAverage();
			return Value;
		}
	}
}
