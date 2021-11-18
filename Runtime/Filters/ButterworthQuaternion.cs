using UnityEngine;

namespace cmdwtf.UnityTools.Filters
{
	public class ButterworthQuaternion : Butterworth<Quaternion>
	{
		public ButterworthQuaternion(float frequency,
										int sampleRate,
										ButterworthPassType passType = ButterworthPassType.Lowpass,
										float resonance = Butterworth.ResonanceMax)
			: base(4, frequency, sampleRate, passType, resonance)
		{ }

		public override Quaternion Sample(Quaternion sample)
		{
			UpdateFilters(sample.x, sample.y, sample.z, sample.w);
			return Value;
		}

		public override Quaternion Value => new Quaternion(
			Filters[0].Value,
			Filters[1].Value,
			Filters[2].Value,
			Filters[3].Value
		);
	}
}
