using UnityEngine;

namespace cmdwtf.UnityTools.Filters
{
	public class ButterworthVector4 : Butterworth<Vector4>
	{
		public ButterworthVector4(float frequency,
									 int sampleRate,
									 ButterworthPassType passType = ButterworthPassType.Lowpass,
									 float resonance = Butterworth.ResonanceMax)
			: base(4, frequency, sampleRate, passType, resonance)
		{ }

		public override Vector4 Sample(Vector4 sample)
		{
			UpdateFilters(sample.x, sample.y, sample.z, sample.w);
			return Value;
		}

		public override Vector4 Value => new Vector4(
			Filters[0].Value,
			Filters[1].Value,
			Filters[2].Value,
			Filters[3].Value
		);
	}
}
