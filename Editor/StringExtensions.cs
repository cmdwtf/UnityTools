using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	public static class StringExtensions
	{
		public static GUIContent ToContent(this string s) => new GUIContent(s, s);
	}
}
