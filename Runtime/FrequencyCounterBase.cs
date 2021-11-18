namespace cmdwtf.UnityTools
{
	public abstract class FrequencyCounterBase : IFrequencyCounter
	{
		protected readonly ITimeProvider Time;
		public virtual int Samples { get; }
		public virtual float Frequency { get; }

		public virtual float Period
		{
			get => Time.PeriodF;
			set => Time.PeriodF = value;
		}

		public virtual float MeasuredSeconds { get; }

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
