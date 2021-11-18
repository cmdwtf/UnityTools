using System.Collections.Generic;
using System.Linq;

namespace cmdwtf.UnityTools.Filters
{
	public class ButterworthMultiple : Butterworth<IEnumerable<float>>, IFilterMultiple<float>
	{
		public ButterworthMultiple(int count,
								   float frequency,
								   int sampleRate,
								   ButterworthPassType passType = ButterworthPassType.Lowpass,
								   float resonance = Butterworth.ResonanceMax)
			: base(count, frequency, sampleRate, passType, resonance)
		{ }

		public IEnumerable<float> Sample(params float[] samples)
			=> UpdateFilters(samples);

		public override IEnumerable<float> Sample(IEnumerable<float> samples)
			=> UpdateFilters(samples.ToArray());

		public override IEnumerable<float> Value => Filters.Select(f => f.Value);
	}
}
