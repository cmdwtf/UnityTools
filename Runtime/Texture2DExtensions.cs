using UnityEngine;

namespace cmdwtf.UnityTools
{
	/// <summary>
	/// Extension methods for dealing with <see cref="Texture2D" />
	/// </summary>
	public static class Texture2DExtensions
	{
		/// <summary>
		/// Fills a texture with a color.
		/// </summary>
		/// <param name="texture">The texture to fill.</param>
		/// <param name="color">The color to fill the texture with.</param>
		public static void Fill(this Texture2D texture, Color color)
		{
			for (int x = 0; x < texture.width; ++x)
			{
				for (int y = 0; y < texture.height; ++y)
				{
					texture.SetPixel(x, y, color);
				}
			}
		}

		/// <summary>
		/// Draws a line from p1 to p2 in the texture.
		/// </summary>
		/// <param name="texture">The texture to draw the line into.</param>
		/// <param name="p1">The start point of the line.</param>
		/// <param name="p2">The end point of the line.</param>
		/// <param name="color">The color of the drawn line.</param>
		public static void DrawLine(this Texture2D texture, Vector2 p1, Vector2 p2, Color color)
		{
			Vector2 t = p1;
			float stepSize = 1/Mathf.Sqrt (Mathf.Pow (p2.x - p1.x, 2) + Mathf.Pow (p2.y - p1.y, 2));
			float ctr = 0;

			while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
			{
				t = Vector2.Lerp(p1, p2, ctr);
				ctr += stepSize;
				texture.SetPixel((int)t.x, (int)t.y, color);
			}
		}

		/// <summary>
		/// Draws a filled circle into the texture.
		/// </summary>
		/// <param name="texture">The texture to draw into.</param>
		/// <param name="x">The horizontal position of the circle.</param>
		/// <param name="y">The vertical position of the circle.</param>
		/// <param name="color">The color of the circle.</param>
		/// <param name="radius">The radius of the circle.</param>
		public static void DrawCircle(this Texture2D texture, int x, int y, Color color, int radius)
		{
			float rSquared = radius * radius;

			for (int u = x - radius; u < x + radius + 1; ++u)
			{
				for (int v = y - radius; v < y + radius + 1; ++v)
				{
					if (((x - u) * (x - u)) + ((y - v) * (y - v)) < rSquared)
					{
						texture.SetPixel(u, v, color);
					}
				}
			}
		}
	}
}
