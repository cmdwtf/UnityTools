using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	public static class RectExtensions
	{
		public static void EditorGUINextLine(this ref Rect r, int numberOfLines = 1)
		{
			float yDelta = EditorGUIUtility.singleLineHeight * numberOfLines;
			float yPadding = EditorGUIUtility.standardVerticalSpacing * numberOfLines;
			
			r.y += yDelta + yPadding;
			r.height -= yDelta;
		}
		
		public static Rect EditorGUILineHeight(this Rect r, int numberOfLines = 1)
		{
			float yDelta = EditorGUIUtility.singleLineHeight * numberOfLines;
			float yPadding = EditorGUIUtility.standardVerticalSpacing * numberOfLines;
			
			r.height = yDelta;
			return r;
		}
	}
}
