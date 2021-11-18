namespace cmdwtf.UnityTools
{
	public abstract class FrequencyCounterBase : IFrequencyCounter
	{
		protected readonly ITimeProvider Time;
		public virtual int Samples { get; protected set; }
		public virtual float Frequency { get; protected set; }

		public virtual float Period
		{
			get => Time.PeriodF;
			set
			{
				Time.PeriodF = value;
				Reset();
			}
		}

		public virtual float MeasuredSeconds { get; protected set; }

		public FrequencyCounterBase(ITimeProvider timeProvider = null, float period = float.NaN)
		{
			if (float.IsNaN(period) || period < 0)
			{
				period = 1f;
			}

			Time = timeProvider ?? TimeProviders.GetDefault(period);
		}

		public virtual void TickAndUpdate()
		{
			Tick();
			Update();
		}

		public abstract void Reset();

		public abstract void Tick();

		public abstract void Update();
	}
}
