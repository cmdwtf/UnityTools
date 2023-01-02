using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class ColorExtensions
	{
		/// <summary>
		/// Returns a new color with the same RGB values and modified alpha.
		/// </summary>
		/// <param name="c">The color to use the RGB values from.</param>
		/// <param name="a">The alpha value the result should have.</param>
		/// <returns>The new color with modified alpha.</returns>
		public static Color WithAlpha(this Color c, float a)
			=> new(c.r, c.g, c.b, a);

		/// <summary>
		/// Creates a <see cref="Texture2D"/> with a size of 1x1 using the current color.
		/// </summary>
		/// <param name="c">The color to create the texture of.</param>
		/// <returns>The created <see cref="Texture2D"/>.</returns>
		public static Texture2D ToTexture2DPixel(this Color c)
		{
			Texture2D texture = new(1, 1);
			texture.SetPixel(0, 0, c);
			texture.Apply();
			return texture;
		}
	}
}
