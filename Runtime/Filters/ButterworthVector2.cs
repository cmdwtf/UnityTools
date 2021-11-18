using UnityEngine;

namespace cmdwtf.UnityTools.Filters
{
	public class ButterworthVector2 : Butterworth<Vector2>
	{
		protected ButterworthVector2(float frequency,
									 int sampleRate,
									 ButterworthPassType passType = ButterworthPassType.Lowpass,
									 float resonance = Butterworth.ResonanceMax)
			: base(2, frequency, sampleRate, passType, resonance)
		{ }

		public override Vector2 Sample(Vector2 sample)
		{
			UpdateFilters(sample.x, sample.y);
			return Value;
		}

		public override Vector2 Value => new Vector2(
			Filters[0].Value,
			Filters[1].Value
			);
	}
}
