using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	public static class RectExtensions
	{
		public static void EditorGUINextLine(this ref Rect r, int numberOfLines = 1, bool centerY = false)
		{
			float yDelta = EditorGUIUtility.singleLineHeight * numberOfLines;
			float yPadding = EditorGUIUtility.standardVerticalSpacing * numberOfLines;

			r.y += yDelta + yPadding;
			r.height -= yDelta;
		}

		public static Rect EditorGUILineHeight(this Rect r, int numberOfLines = 1, bool centerY = false)
		{
			float yDelta = EditorGUIUtility.singleLineHeight * numberOfLines;
			float yPadding = EditorGUIUtility.standardVerticalSpacing * Mathf.Max(0, numberOfLines - 1);

			r.height = yDelta + yPadding;
			r.y += yPadding;

			if (centerY)
			{
				r.y += yDelta / 2f;
			}

			return r;
		}

		public static Rect EditorGUICenterVertical(this Rect r)
		{
			float yDelta = EditorGUIUtility.singleLineHeight;
			float yPadding = EditorGUIUtility.standardVerticalSpacing;

			r.y += (yDelta + yPadding) / 2f;

			return r;
		}

		public static Rect EditorGUILineHeightTabs(this Rect r, int tabCount, float tabPadding = 0f, int numberOfLines = 1)
		{
			Rect result = r.EditorGUILineHeight(numberOfLines);

			float tabWidth = result.width / tabCount;
			result.width = tabWidth;

			float halfPadding = tabPadding / 2f;
			result.x += halfPadding;
			result.width -= tabPadding;

			return result;
		}

		public static void EditorGUINextTab(this ref Rect r,
											float tabPadding = 0f
		)
		{
			float halfPadding = tabPadding / 2f;
			r.x += r.width + tabPadding;
		}

	}
}
