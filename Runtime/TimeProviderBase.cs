using System;

namespace cmdwtf.UnityTools
{
	public abstract class TimeProviderBase : ITimeProvider
	{
		public double Period { get; set; }

		public float PeriodF
		{
			get => (float)Period;
			set => Period = value;
		}

		public abstract double NowSeconds { get; }
		public virtual float NowSecondsF => (float)NowSeconds;

		public virtual double DeltaSeconds { get; protected set; }
		public virtual float DeltaSecondsF => (float)DeltaSeconds;
		public abstract long UnixTimestamp { get; }

		protected double LastUpdate { get; set; } = double.NaN;

		public TimeProviderBase()
			: this(0d)
		{ }

		public TimeProviderBase(double period = 0)
		{
			Period = period;
		}

		public TimeProviderBase(float periodF = 0)
		{
			PeriodF = periodF;
		}

		public virtual double Update()
		{
			double now = NowSeconds;

			if (double.IsNaN(LastUpdate))
			{
				LastUpdate = now;
				return 0;
			}

			DeltaSeconds  = now - LastUpdate;
			LastUpdate = now;
			return DeltaSeconds;
		}

		public virtual float UpdateF() => (float)Update();
	}
}
