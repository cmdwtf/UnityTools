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

		public static bool IsFiniteOrZero(this float f)
			=> float.IsFinite(f) || f == 0;

		public static string ToStringPercent(this double d)
			=> $"{d * 100.0:0.00}%";

		public static string ToString2Points(this double d)
			=> $"{d:0.00}";

		public static string ToString0Points(this double d)
			=> $"{System.Math.Truncate(d):0.}";

		public static bool IsFiniteOrZero(this double d)
			=> double.IsFinite(d) || d == 0;

		public static string ToStringPercent(this decimal d)
			=> $"{d * 100.0m:0.00}%";

		public static string ToString2Points(this decimal d)
			=> $"{d:0.00}";

		public static string ToString0Points(this decimal d)
			=> $"{System.Math.Truncate(d):0.}";

		public static bool IsFiniteOrZero(this decimal f)
			=> true;
	}
}
