using System;

namespace cmdwtf.UnityTools
{
	public class DotNetTimeProvider : TimeProviderBase
	{
		protected readonly TimeSpan Epoch = new TimeSpan(new DateTime(1970, 1, 1).Ticks);

		public override double NowSeconds => DateTimeOffset.Now.TimeOfDay.TotalSeconds;

		public override long UnixTimestamp
		{
			get
			{
				TimeSpan unixTicks = new TimeSpan(DateTimeOffset.UtcNow.Ticks) - Epoch;
				return (long) System.Math.Round(unixTicks.TotalSeconds);
			}
		}

		public DotNetTimeProvider() { }

		public DotNetTimeProvider(double period = 0)
			: base(period) { }

		public DotNetTimeProvider(float periodF = 0)
			: base(periodF) { }
	}
}
