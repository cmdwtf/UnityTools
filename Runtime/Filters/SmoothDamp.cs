namespace cmdwtf.UnityTools.Filters
{
	public class SmoothDamp : TimeBasedMovingAverage
	{
		public float MaxSpeed { get; set; } = float.PositiveInfinity;

		private float _velocity = 0;

		public SmoothDamp(int startupWindow, ITimeProvider timeProvider)
			: base(startupWindow, timeProvider) { }

		public SmoothDamp(int startupWindow, float period, float minimumDeltaTime = 0f)
			: base(startupWindow, period, minimumDeltaTime) { }

		public override float Sample(float sample)
		{
			UpdateTime();
			if (!WarmedUp)
			{
				return base.Sample(sample);
			}

			Value = DoMath(Value, sample, ref _velocity, Time.PeriodF, MaxSpeed, DeltaTime);
			return Value;
		}


		// Gradually changes a value towards a desired goal over time.
		protected static float DoMath(float current,
											  float target,
											  ref float currentVelocity,
											  float smoothTime,
											  float maxSpeed = float.PositiveInfinity,
											  float deltaTime = 0/*Time.deltaTime*/
		)
		{
			// Based on Game Programming Gems 4 Chapter 1.10
			smoothTime = System.Math.Max(0.0001F, smoothTime);
			float omega = 2F / smoothTime;

			float x = omega * deltaTime;
			float exp = 1F / (1F + x + (0.48F * x * x) + (0.235F * x * x * x));
			float change = current - target;
			float originalTo = target;

			// Clamp maximum speed
			float maxChange = maxSpeed * smoothTime;
			change = Math.Clamp(change, -maxChange, maxChange);
			target = current - change;

			float temp = (currentVelocity + (omega * change)) * deltaTime;
			currentVelocity = (currentVelocity - (omega * temp)) * exp;
			float output = target + ((change + temp) * exp);

			// Prevent overshooting
			if ((originalTo - current > 0.0F) == (output > originalTo))
			{
				output = originalTo;
				currentVelocity = (output - originalTo) / deltaTime;
			}

			return output;
		}
	}
}
