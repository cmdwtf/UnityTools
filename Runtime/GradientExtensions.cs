using System;

using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class GradientExtensions
	{
		/// <summary>
		/// Duplicates the prototype <see cref="Gradient"/>'s color and alpha keys, returning a new gradient with identical values.
		/// </summary>
		/// <param name="prototype">The <see cref="Gradient"/> to copy.</param>
		/// <returns>The newly created duplicate.</returns>
		public static Gradient Clone(this Gradient prototype)
		{
			Gradient result = new();

			if (prototype == null)
			{
				return result;
			}

			result.mode = prototype.mode;

			var colorKeys = new GradientColorKey[prototype.colorKeys.Length];
			var alphaKeys = new GradientAlphaKey[prototype.alphaKeys.Length];

			Array.Copy(prototype.colorKeys, colorKeys, colorKeys.Length);
			Array.Copy(prototype.alphaKeys, alphaKeys, alphaKeys.Length);

			result.SetKeys(colorKeys, alphaKeys);

			return result;
		}
	}
}
