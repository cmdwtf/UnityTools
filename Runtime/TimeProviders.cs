namespace cmdwtf.UnityTools
{
	public static class TimeProviders
	{
#if UNITY
		public static ITimeProvider GetDefault(double period = 1d) => GetUnityTimeProvider(period);
		public static ITimeProvider GetDefault(float period) => GetUnityTimeProvider(period);
		public static UnityTimeProvider GetUnityTimeProvider(double period = 0d) => new UnityTimeProvider(period);
		public static UnityTimeProvider GetUnityTimeProvider(float period) => new UnityTimeProvider(period);
#else
		public static ITimeProvider GetDefault(double period = 1d) => GetDotNetTimeProvider(period);
		public static ITimeProvider GetDefault(float period) => GetDotNetTimeProvider(period);
		public static DotNetTimeProvider GetDotNetTimeProvider(double period = 0d) => new DotNetTimeProvider(period);
		public static DotNetTimeProvider GetDotNetTimeProvider(float period) => new DotNetTimeProvider(period);
#endif // UNITY
	}
}
