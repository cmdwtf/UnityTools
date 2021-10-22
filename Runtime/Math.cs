using UnityEngine;

namespace cmdwtf.UnityTools
{
	public class Math
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
		
		public static Vector3 SmoothDamp(Vector3 cur, Vector3 tgt, ref Vector3 velocity, float time)
		{
			Vector3 ret = Vector3.zero;
			ret.x = Mathf.SmoothDamp(cur.x, tgt.x, ref velocity.x, time);
			ret.y = Mathf.SmoothDamp(cur.y, tgt.y, ref velocity.y, time);
			ret.z = Mathf.SmoothDamp(cur.z, tgt.z, ref velocity.z, time);
			return ret;
		}

	}
}
