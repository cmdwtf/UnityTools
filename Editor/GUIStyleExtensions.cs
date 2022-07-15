using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	public static class GUIStyleExtensions
	{
		public static GUIStyle UpperLeft(this GUIStyle s) => s.WithAnchor(TextAnchor.UpperLeft);
		public static GUIStyle UpperCenter(this GUIStyle s) => s.WithAnchor(TextAnchor.UpperCenter);
		public static GUIStyle UpperRight(this GUIStyle s) => s.WithAnchor(TextAnchor.UpperRight);
		public static GUIStyle MiddleLeft(this GUIStyle s) => s.WithAnchor(TextAnchor.MiddleLeft);
		public static GUIStyle MiddleCenter(this GUIStyle s) => s.WithAnchor(TextAnchor.MiddleCenter);
		public static GUIStyle MiddleRight(this GUIStyle s) => s.WithAnchor(TextAnchor.MiddleRight);
		public static GUIStyle LowerLeft(this GUIStyle s) => s.WithAnchor(TextAnchor.LowerLeft);
		public static GUIStyle LowerCenter(this GUIStyle s) => s.WithAnchor(TextAnchor.LowerCenter);
		public static GUIStyle LowerRight(this GUIStyle s) => s.WithAnchor(TextAnchor.LowerRight);

		public static GUIStyle Left(this GUIStyle s) => s.WithAnchor(TextAnchor.MiddleLeft);
		public static GUIStyle Center(this GUIStyle s) => s.WithAnchor(TextAnchor.MiddleCenter);
		public static GUIStyle Right(this GUIStyle s) => s.WithAnchor(TextAnchor.MiddleRight);


		private static GUIStyle WithAnchor(this GUIStyle s, TextAnchor align)
			=> new(s) { alignment = align,};
	}
}
