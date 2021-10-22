using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class WWWExtensions
	{
#pragma warning disable 0618
		public static bool HasError(this WWW w, string errorStringPrefix = "ERROR")
			=> string.IsNullOrEmpty(w.error) == false ||
			   w.text.StartsWith(errorStringPrefix);
#pragma warning restore 0618
	}
}
