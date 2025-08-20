using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class Math
	{
		private const float DegreesInCircle = 360.0f;
		private const float DegreesInHalfCircle = 180.0f;

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
			float percent = (t - min) / distance;

			if (t < min)
			{
				percent = MakeNegative(percent);
			}

			return percent;
		}

		public static float LerpUnclamped(float from, float to, float t)
			=> from + ((to - from) * t);

		public static Vector2 LerpUnclamped(Vector2 from, Vector2 to, Vector2 t)
			=> new Vector2(
				LerpUnclamped(from.x, to.x, t.x),
				LerpUnclamped(from.y, to.y, t.y)
			);

		public static Vector3 LerpUnclamped(Vector3 from, Vector3 to, Vector3 t)
			=> new Vector3(
				LerpUnclamped(from.x, to.x, t.x),
				LerpUnclamped(from.y, to.y, t.y),
				LerpUnclamped(from.z, to.z, t.z)
			);

		public static Vector4 LerpUnclamped(Vector4 from, Vector4 to, Vector4 t)
			=> new Vector4(
				LerpUnclamped(from.x, to.x, t.x),
				LerpUnclamped(from.y, to.y, t.y),
				LerpUnclamped(from.z, to.z, t.z),
				LerpUnclamped(from.w, to.w, t.w)
			);

		public static float InverseLerpUnclamped(float from, float to, float t)
			=> (t - from) / (to - from);

		public static Vector2 InverseLerpUnclamped(Vector2 from, Vector2 to, Vector2 t)
			=> new Vector2(
				InverseLerpUnclamped(from.x, to.x, t.x),
				InverseLerpUnclamped(from.y, to.y, t.y)
			);

		public static Vector3 InverseLerpUnclamped(Vector3 from, Vector3 to, Vector3 t)
			=> new Vector3(
				InverseLerpUnclamped(from.x, to.x, t.x),
				InverseLerpUnclamped(from.y, to.y, t.y),
				InverseLerpUnclamped(from.z, to.z, t.z)
			);

		public static Vector4 InverseLerpUnclamped(Vector4 from, Vector4 to, Vector4 t)
			=> new Vector4(
				InverseLerpUnclamped(from.x, to.x, t.x),
				InverseLerpUnclamped(from.y, to.y, t.y),
				InverseLerpUnclamped(from.z, to.z, t.z),
				InverseLerpUnclamped(from.w, to.w, t.w)
			);

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

		/// <summary>
		/// Wraps <paramref name="angle"/> to the range of (-180, 180].
		/// </summary>
		/// <param name="angle">The angle to wrap</param>
		/// <returns>The angle value wrapped to (-180, 180]</returns>
		public static float NormalizeAngle(float angle)
		{
			float offset = (Mathf.Ceil((angle + DegreesInHalfCircle) / DegreesInCircle) - 1.0f) * DegreesInCircle;
			return angle - offset;
		}

		public static short KeepIn360Degrees(short angle)
			=> Repeat(angle, (short)360);
		public static int KeepIn360Degrees(int angle)
			=> Repeat(angle, 360);
		public static long KeepIn360Degrees(long angle)
			=> Repeat(angle, 360);
		public static float KeepIn360Degrees(float angle)
			=> Repeat(angle, 360.0f);

		public static short Repeat(short t, short length)
			=> (short)Mathf.Repeat(t, length);
		public static int Repeat(int t, int length)
			=> (int)Mathf.Repeat(t, length);
		public static long Repeat(long t, long length)
			=> (long)Mathf.Repeat(t, length);
		public static float Repeat(float t, float length)
			=> Mathf.Repeat(t, length);

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

		public static float SmoothMax(float a, float b, float k)
			=> Mathf.Log(Mathf.Exp(k * a) + Mathf.Exp(k * b)) / k;

		public static float SmoothMin(float a, float b, float k)
			=> -SmoothMax(-a, -b, k);

		public static int ClosestMultiple(int x, int y)
			=> (int)Mathf.Round((float)x / y) * y;

		public static float ClosestMultiple(float x, float y)
			=> Mathf.Round(x / y) * y;

		public static double ClosestMultiple(double x, double y)
			=> System.Math.Round(x / y) * y;

		public static Vector3 RandomVector(float min, float max)
			=> new Vector3(Random.Range(min, max), Random.Range(min, max), Random.Range(min, max));

		public static float Remap(float inMin, float inMax, float outMin, float outMax, float value)
		{
			float inT = Mathf.InverseLerp(inMin, inMax, value);
			return Mathf.Lerp(outMin, outMax, inT);
		}

		public static float RemapUnclamped(float inMin, float inMax, float outMin, float outMax, float value)
		{
			float inT = Mathf.InverseLerp(inMin, inMax, value);
			return Mathf.LerpUnclamped(outMin, outMax, inT);
		}

		internal static float FlatAngle(Vector2 v)
		{
			float angle = Vector2.SignedAngle(v, Vector2.up);
			return (angle + 360.0f) % 360.0f;
		}

		internal static float FlatAngle(Vector3 v)
		{
			var flattened = new Vector2(v.x, v.z);
			float angle = Vector2.SignedAngle(flattened, Vector2.up);
			angle += 180.0f;
			return angle % 360.0f;
		}
	}
}
