using UnityEngine;

namespace cmdwtf.UnityTools.Filters
{
	public class ButterworthVector3 : Butterworth<Vector3>
	{
		protected ButterworthVector3(float frequency,
									 int sampleRate,
									 ButterworthPassType passType = ButterworthPassType.Lowpass,
									 float resonance = Butterworth.ResonanceMax)
			: base(3, frequency, sampleRate, passType, resonance)
		{ }

		public override Vector3 Sample(Vector3 sample)
		{
			UpdateFilters(sample.x, sample.y, sample.z);
			return Value;
		}

		public override Vector3 Value => new Vector3(
			Filters[0].Value,
			Filters[1].Value,
			Filters[2].Value
		);
	}
}
