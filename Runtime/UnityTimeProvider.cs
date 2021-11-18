using UnityEngine;

#if UNITY
namespace cmdwtf.UnityTools
{
	public class UnityTimeProvider : TimeProviderBase
	{
		protected readonly DotNetTimeProvider DotNetTime = new DotNetTimeProvider();
		public override double NowSeconds => Time.realtimeSinceStartupAsDouble;
		public override float NowSecondsF => Time.realtimeSinceStartup;
		public override long UnixTimestamp => DotNetTime.UnixTimestamp;

		public UnityTimeProvider() { }

		public UnityTimeProvider(double period = 0)
			: base(period) { }

		public UnityTimeProvider(float periodF = 0)
			: base(periodF) { }
	}
}
#endif // UNITY
