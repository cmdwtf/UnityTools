namespace cmdwtf.UnityTools
{
	public static class NumberExtensions
	{
		public static string ToStringPercent(this float f)
			=> $"{f * 100.0f:0.00}%";

		public static string ToString2Points(this float f)
			=> $"{f:0.00}";

		public static string ToString0Points(this float f)
			=> $"{System.Math.Truncate(f):0.}";
	}
}
