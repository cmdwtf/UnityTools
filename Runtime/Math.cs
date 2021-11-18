using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class Math
	{
		public static sbyte MakePositive(sbyte b) => (sbyte)(b < 0 ? b * -1 : b);
		public static short MakePositive(short s) => (short)(s < 0 ? s * -1 : s);
		public static int MakePositive(int i) => i < 0 ? i * -1 : i;
		public static long MakePositive(long l) => l < 0 ? l * -1 : l;
		public static float MakePositive(float f) => f < 0 ? f * -1 : f;
		public static double MakePositive(double d) => d < 0 ? d * -1 : d;
		public static decimal MakePositive(decimal m) => m < 0 ? m * -1 : m;

		public static sbyte MakeNegative(sbyte b) => (sbyte)(b < 0 ? b * -1 : b);
		public static short MakeNegative(short s) => (short)(s < 0 ? s * -1 : s);
		public static int MakeNegative(int i) => i < 0 ? i * -1 : i;
		public static long MakeNegative(long l) => l < 0 ? l * -1 : l;
		public static float MakeNegative(float f) => f < 0 ? f * -1 : f;
		public static double MakeNegative(double d) => d < 0 ? d * -1 : d;
		public static decimal MakeNegative(decimal m) => m < 0 ? m * -1 : m;

		public static float PercentOf(float min, float max, float t)
		{
			float distance = max - min;
			float percent = (t-min)/distance;

			if (t < min)
			{
				percent = MakeNegative(percent);
			}

			return percent;
		}

		public static float LerpNoClamp(float from, float to, float t)
			=> from + ((to - from) * t);

		public static bool GreaterThanEpsilon(float a, float b, float epsilon)
			=> a > b
			   || a + epsilon > b
			   || a - epsilon > b;

		public static bool LessThanEpsilon(float a, float b, float epsilon)
			=> a < b
			   || a + epsilon < b
			   || a - epsilon < b;

		public static bool GreaterThanEqualEpsilon(float a, float b, float epsilon)
			=> a >= b
			   || a + epsilon >= b
			   || a - epsilon >= b;

		public static bool LessThanEqualEpsilon(float a, float b, float epsilon)
			=> a <= b
			   || a + epsilon <= b
			   || a - epsilon <= b;

		public static short KeepIn360Degrees(short angle)
			=> Repeat(angle, (short)360);
		public static int KeepIn360Degrees(int angle)
			=> Repeat(angle, 360);
		public static long KeepIn360Degrees(long angle)
			=> Repeat(angle, 360);
		public static short Repeat(short t, short length)
			=> (short)Mathf.Repeat(t, length);
		public static int Repeat(int t, int length)
			=> (int)Mathf.Repeat(t, length);
		public static long Repeat(long t, long length)
			=> (long)Mathf.Repeat(t, length);

		public static float SmoothStep(float from, float to, float t)
			// this exists in the unity math library
			=> Mathf.SmoothStep(from, to, t);

		public static double SmoothStep(double from, double to, double t)
		{
			t = t.Clamp01();
			t = (-2.0 * t * t * t) + (3.0 * t * t);
			return (to * t) + (@from * (1.0 - t));
		}

		public static decimal SmoothStep(decimal from, decimal to, decimal t)
		{
			t = t.Clamp01();
			t = (-2.0m * t * t * t) + (3.0m * t * t);
			return (to * t) + (@from * (1.0m - t));
		}

		public static Vector2 SmoothDamp(Vector2 cur, Vector2 tgt, ref Vector2 velocity, float time)
		{
			Vector2 ret = Vector2.zero;
			ret.x = Mathf.SmoothDamp(cur.x, tgt.x, ref velocity.x, time);
			ret.y = Mathf.SmoothDamp(cur.y, tgt.y, ref velocity.y, time);
			return ret;
		}

		public static Vector3 SmoothDamp(Vector3 cur, Vector3 tgt, ref Vector3 velocity, float time)
		{
			Vector3 ret = Vector3.zero;
			ret.x = Mathf.SmoothDamp(cur.x, tgt.x, ref velocity.x, time);
			ret.y = Mathf.SmoothDamp(cur.y, tgt.y, ref velocity.y, time);
			ret.z = Mathf.SmoothDamp(cur.z, tgt.z, ref velocity.z, time);
			return ret;
		}

		public static Vector4 SmoothDamp(Vector4 cur, Vector4 tgt, ref Vector4 velocity, float time)
		{
			Vector4 ret = Vector4.zero;
			ret.x = Mathf.SmoothDamp(cur.x, tgt.x, ref velocity.x, time);
			ret.y = Mathf.SmoothDamp(cur.y, tgt.y, ref velocity.y, time);
			ret.z = Mathf.SmoothDamp(cur.z, tgt.z, ref velocity.z, time);
			ret.w = Mathf.SmoothDamp(cur.w, tgt.w, ref velocity.w, time);
			return ret;
		}

		public static int ClosestMultiple(int x, int y)
			=> (int)Mathf.Round((float)x/y) * y;

		public static float ClosestMultiple(float x, float y)
			=> Mathf.Round(x/y) * y;

		public static double ClosestMultiple(double x, double y)
			=> System.Math.Round(x/y) * y;

		public static Vector3 RandomVector(float min, float max)
			=> new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));
	}
}
