
namespace cmdwtf.UnityTools.Filters
{
	public class ExponentialMovingAverageTimeBased : TimeBasedMovingAverage
	{
		public float Smoothness { get; private set; }

		public ExponentialMovingAverageTimeBased(int startupWindow, float smoothness, ITimeProvider timeProvider)
			: base(startupWindow, timeProvider)
		{
			Smoothness = smoothness;
		}

		public ExponentialMovingAverageTimeBased(int warmupWindow, float smoothness, float period, float minimumDeltaTime = 0f)
			: base(warmupWindow, period, minimumDeltaTime)
		{
			Smoothness = smoothness;
		}

		public override float Sample(float sample)
		{
			UpdateTime();

			if (!WarmedUp)
			{
				return base.Sample(sample);
			}

			float periodPercent = DeltaTime / Time.PeriodF;

			float newSampleWeight = periodPercent / (periodPercent + Smoothness);
			float oldSampleWeight = 1 - newSampleWeight;

			Value = (sample * newSampleWeight) + (Value * oldSampleWeight);
			return Value;

			//float k = Period / (Smoothness + 1);
			//Value = (sample * k) + (Value * (1 - k));
			//return Value;
		}
	}
}
