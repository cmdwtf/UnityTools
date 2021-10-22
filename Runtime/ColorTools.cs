using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class ColorTools
	{
		public static Color SmoothDamp(Color one, Color two, ref float lerpVal, ref float lerpMult, float speed, float minPerc)
		{
			lerpVal += Time.deltaTime * speed * lerpMult;
			if (lerpVal > 1)
			{
				lerpVal = 1;
				lerpMult *= -1;
			}
			else if (lerpVal < minPerc)
			{
				lerpVal = minPerc;
				lerpMult *= -1;
			}

			return Color.Lerp(two, one, lerpVal);
		}
	}
}
