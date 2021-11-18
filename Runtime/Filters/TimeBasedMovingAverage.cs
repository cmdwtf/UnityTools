namespace cmdwtf.UnityTools.Filters
{
	public abstract class TimeBasedMovingAverage : VirtualWindowAverage
	{
		protected ITimeProvider Time { get; set; }
		protected float MinimumDeltaTime { get; set; }
		protected float DeltaTime { get; private set; }

		public TimeBasedMovingAverage(int startupWindow, ITimeProvider timeProvider = null)
			: base(startupWindow, 1f)
		{
			Time = timeProvider ?? TimeProviders.GetDefault(1f);
		}

		public TimeBasedMovingAverage(int startupWindow, float period, float minimumDeltaTime = float.NaN)
			: base(startupWindow, period)
		{
			Time = TimeProviders.GetDefault(period);
			MinimumDeltaTime = float.IsNaN(minimumDeltaTime) ? period : minimumDeltaTime;
		}

		protected void UpdateTime()
		{
			DeltaTime = Time.UpdateF();

			if (DeltaTime < MinimumDeltaTime)
			{
				DeltaTime = MinimumDeltaTime;
			}
		}
	}
}
