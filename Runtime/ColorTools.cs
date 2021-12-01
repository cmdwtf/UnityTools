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

		public static Color FromBytes(byte r, byte g, byte b, byte a = 255)
		{
			byte[] rgba = { r, g, b, a };
			return FromBytes(rgba);
		}

		public static Color FromBytes(params byte[] rgba)
		{
			int count = rgba.Length;
			float r = count >= 1 ? (rgba[0] / 255f) : 0f;
			float g = count >= 2 ? (rgba[1] / 255f) : 0f;
			float b = count >= 3 ? (rgba[2] / 255f) : 0f;
			float a = count >= 4 ? (rgba[3] / 255f) : 1f;

			return new Color(r, g, b, a);
		}
	}
}
