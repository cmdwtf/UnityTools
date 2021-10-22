using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class MathExtensions
	{
		public static float ClampTo(this float f, float min, float max)
			=> Mathf.Clamp(f, min, max);

		public static Vector3 RandomVector(float min, float max)
			=> new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));

		public static float MakePositive(this float f)
			=> Math.MakePositive(f);

		public static float MakeNegitive(this float f)
			=> Math.MakeNegative(f);

		public static double MakePositive(this double f)
			=> Math.MakePositive(f);

		public static double MakeNegitive(this double f)
			=> Math.MakeNegative(f);
	}
}
