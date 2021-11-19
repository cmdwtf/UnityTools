using System;
using System.Collections;

namespace cmdwtf.UnityTools.Filters
{
	public abstract class Butterworth<T> : FilterBase<T>, IBiQuadFilter<T>
	{
		public float Resonance => First.Resonance;
		public float Frequency => First.Frequency;
		public int SampleRate => First.SampleRate;
		public override FilterType Type => First.Type;
		public override string Name => First.Name;

		protected Butterworth[] Filters { get; }

		private Butterworth First => Filters[0];

		protected Butterworth(int componentCount, float frequency, int sampleRate, ButterworthPassType passType, float resonance)
		{
			Filters = new Butterworth[componentCount];
			for (int scan = 0; scan < componentCount; ++scan)
			{
				Filters[scan] = new Butterworth(frequency, sampleRate, passType, resonance);
			}
		}

		public override void Reset()
		{
			foreach (Butterworth f in Filters)
			{
				f.Reset();
			}
		}

		public T UpdateFilters(params float[] samples)
		{
			if (samples.Length != Filters.Length)
			{
				throw new ArgumentException(
					$"{nameof(UpdateFilters)} expects a number of parameters to match the number of filters.");
			}

			for (int scan = 0; scan < Filters.Length; ++scan)
			{
				Filters[scan].Sample(samples[scan]);
			}

			return Value;
		}
	}
}
