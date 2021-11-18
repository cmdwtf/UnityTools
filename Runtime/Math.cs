using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class Math
	{
		public static float MakePositive(float f)
			=> f < 0 ? f * -1 : f;

		public static float MakeNegative(float f)
			=> f > 0 ? f * -1 : f;

		public static double MakePositive(double f)
			=> f < 0 ? f * -1 : f;

		public static double MakeNegative(double f)
			=> f > 0 ? f * -1 : f;

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

		public static short KeepIn360Degrees(short s)
		{
			while (s >= 360)
			{
				s -= 360;
			}

			while (s < 0)
			{
				s += 360;
			}

			return s;
		}

		public static int KeepIn360Degrees(int s)
		{
			while (s >= 360)
			{
				s -= 360;
			}

			while (s < 0)
			{
				s += 360;
			}

			return s;
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

		public static ulong Clamp(this ulong v, ulong min, ulong max)
			=> (v > max ? max : (v < min ? min : v));

		public static uint Clamp(this uint v, uint min, uint max)
			=> (v > max ? max : (v < min ? min : v));

		public static ushort Clamp(this ushort v, ushort min, ushort max)
			=> (v > max ? max : (v < min ? min : v));

		public static float Clamp(this float v, float min, float max)
			=> (v > max ? max : (v < min ? min : v));

		public static sbyte Clamp(this sbyte v, sbyte min, sbyte max)
			=> (v > max ? max : (v < min ? min : v));

		public static long Clamp(this long v, long min, long max)
			=> (v > max ? max : (v < min ? min : v));

		public static int Clamp(this int v, int min, int max)
			=> (v > max ? max : (v < min ? min : v));

		public static short Clamp(this short v, short min, short max)
			=> (v > max ? max : (v < min ? min : v));

		public static double Clamp(this double v, double min, double max)
			=> (v > max ? max : (v < min ? min : v));

		public static decimal Clamp(this decimal v, decimal min, decimal max)
			=> (v > max ? max : (v < min ? min : v));

		public static byte Clamp(this byte v, byte min, byte max)
			=> (v > max ? max : (v < min ? min : v));

		public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max)
			=> new Vector2(
				v.x.Clamp(min.x, min.y),
				v.y.Clamp(min.y, max.y)
			);

		public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max)
			=> new Vector3(
				v.x.Clamp(min.x, min.y),
				v.y.Clamp(min.y, max.y),
				v.z.Clamp(min.z, max.z)
			);

		public static Vector4 Clamp(this Vector4 v, Vector4 min, Vector4 max)
			=> new Vector4(
				v.x.Clamp(min.x, min.y),
				v.y.Clamp(min.y, max.y),
				v.z.Clamp(min.z, max.z),
				v.w.Clamp(min.w, max.w)
			);

		public static short NthTriangular(this short n)
			=> (short)((n * (n + 1)) / 2);

		public static int NthTriangular(this int n)
			=> (n * (n + 1)) / 2;

		public static long NthTriangular(this long n)
			=> (n * (n + 1)) / 2;

		public static float NthTriangular(this float n)
			=> (n * (n + 1)) / 2.0f;

		public static double NthTriangular(this double n)
			=> (n * (n + 1)) / 2.0d;

		public static decimal NthTriangular(this decimal n)
			=> (n * (n + 1)) / 2.0m;
	}
}
