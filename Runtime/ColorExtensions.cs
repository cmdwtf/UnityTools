using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class ColorExtensions
	{
		public static Color WithAlpha(this Color c, float a)
		{
			return new Color(c.r, c.g, c.b, a);
		}
	}
}
