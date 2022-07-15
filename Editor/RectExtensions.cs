using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	/// <summary>
	/// Extension methods for the <see cref="Rect"/> class.
	/// </summary>
	public static class RectExtensions
	{
		public static void Space(this ref Rect r, float pixels) => r.y += pixels;

		public static Rect Width(this Rect r, float width, bool centerX = true)
		{
			float delta = r.width - width;
			r.width = width;

			if (centerX)
			{
				r.x += delta / 2f;
			}

			return r;
		}

		public static Rect WidthScale(this Rect r, float widthScale, bool centerX = true)
			=> r.Width(r.width * widthScale, centerX);

		public static Rect Height(this Rect r, float height, bool centerY = true)
		{
			float delta = r.height - height;
			r.height = height;

			if (centerY)
			{
				r.y += delta / 2f;
			}

			return r;
		}

		public static Rect HeightScale(this Rect r, float heightScale, bool centerY = true)
			=> r.Height(r.height * heightScale, centerY);

		public static Rect CollapseToLeft(this Rect r, float finalWidth)
		{
			r.width = finalWidth;
			return r;
		}

		public static Rect CollapseToRight(this Rect r, float finalWidth)
		{
			float shift = r.width - finalWidth;
			r.x += shift;
			r.width = finalWidth;
			return r;
		}

		public static Rect CollapseToTop(this Rect r, float finalHeight)
		{
			r.height = finalHeight;
			return r;
		}

		public static Rect CollapseToBottom(this Rect r, float finalHeight)
		{
			float shift = r.height - finalHeight;
			r.y += shift;
			r.height = finalHeight;
			return r;
		}

		public static Rect CollapseToLeftPercent(this Rect r, float percent)
			=> r.CollapseToLeft(r.width * percent);

		public static Rect CollapseToRightPercent(this Rect r, float percent)
			=> r.CollapseToRight(r.width * percent);

		public static Rect CollapseToTopPercent(this Rect r, float percent)
			=> r.CollapseToTop(r.height * percent);

		public static Rect CollapseToBottomPercent(this Rect r, float percent)
			=> r.CollapseToBottom(r.height * percent);

		public static Rect BumpUp(this Rect r, float pixels)
			=> r.Bump(-pixels, 0);

		public static Rect BumpDown(this Rect r, float pixels)
			=> r.Bump(pixels, 0);

		public static Rect BumpLeft(this Rect r, float pixels)
			=> r.Bump(0, -pixels);

		public static Rect BumpRight(this Rect r, float pixels)
			=> r.Bump(0, pixels);

		public static Rect Bump(this Rect r, float vertical, float horizontal)
		{
			r.y += vertical;
			r.x += horizontal;
			return r;
		}

		public static Rect IndentedRect(this Rect r, int extraIndents = 0)
		{
			using EditorGUI.IndentLevelScope scope = new(extraIndents);
			return EditorGUI.IndentedRect(r);
		}

		public static void EditorGUINextLine(this ref Rect r, int numberOfLines = 1, bool centerY = false)
		{
			float yDelta = EditorGUIUtility.singleLineHeight * numberOfLines;
			float yPadding = EditorGUIUtility.standardVerticalSpacing * numberOfLines;

			r.y += yDelta + yPadding;
			r.height -= yDelta;
		}

		public static Rect EditorGUILabelWidth(this Rect r, int numberOfLines = 1, bool centerY = false)
		{
			r = r.EditorGUILineHeight(numberOfLines, centerY);
			r.width = EditorGUIUtility.labelWidth;
			return r;
		}

		public static Rect IdEvenKMan(this Rect r, int numberOfLines = 1, bool centerY = false)
		{
			r = r.EditorGUILineHeight(numberOfLines, centerY);
			// so the reported field width is... funny, and often not what we want.
			// instead, we'll find out how much percentage of the line is taken by the label,
			// then use that to multiply into our width, and collapse right that much.
			float labelPercent = (EditorGUIUtility.labelWidth / EditorGUIUtility.currentViewWidth);
			//float fieldPercent = (EditorGUIUtility.fieldWidth / EditorGUIUtility.currentViewWidth);
			//r.x += (r.width * labelPercent);
			//r.width -= (r.width * labelPercent);
			return r.CollapseToRight(r.width - (r.width * labelPercent));

			// the width should be set to the field width.
			// EditorGUIUtility.fieldWidth only reports the *minimum* amount of pixels
			// that the field will have. So instead, just subtract the label width
			// from the full size rect we got from EditorGUILineHeight().
			//r.width -= EditorGUIUtility.labelWidth;
			//r.width = EditorGUIUtility.fieldWidth;
			//return r;
		}

		public static Rect EditorGUIFieldWidth(this Rect r, int numberOfLines = 1, bool centerY = false)
		{
			r = r.EditorGUILineHeight(numberOfLines, centerY);
			r.x += EditorGUIUtility.labelWidth;

			// the width should be set to the field width.
			// EditorGUIUtility.fieldWidth only reports the *minimum* amount of pixels
			// that the field will have. So instead, just subtract the label width
			// from the full size rect we got from EditorGUILineHeight().
			r.width -= EditorGUIUtility.labelWidth;
			return r;
		}

		public static Rect EditorGUILineHeight(this Rect r, int numberOfLines = 1, bool centerY = false)
		{
			float yDelta = EditorGUIUtility.singleLineHeight * numberOfLines;
			r.height = yDelta;

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

		public static Rect Expand(this Rect r, float top, float left, float bottom, float right, bool keepCenter = true)
		{
			float widthShrink = (left + right);
			float heightShrink = (top + bottom);

			r.width += widthShrink;
			r.height += heightShrink;

			if (!keepCenter)
			{
				return r;
			}

			r.x -= widthShrink / 2f;
			r.y -= heightShrink / 2f;

			return r;
		}

		public static Rect Expand(this Rect r, float width, float height, bool keepCenter = true)
			=> r.Expand(height / 2f, width / 2f, height / 2f, width / 2f, keepCenter);

		public static Rect Expand(this Rect r, float amount, bool keepCenter = true)
			=> r.Expand(amount, amount, keepCenter);

		public static Rect Shrink(this Rect r, float amount, bool keepCenter = true)
			=> r.Expand(-amount, -amount, keepCenter);

		public static Rect Shrink(this Rect r, float width, float height, bool keepCenter = true)
			=> r.Expand(-width, -height, keepCenter);

		public static Rect Shrink(this Rect r, float top, float left, float bottom, float right, bool keepCenter = true)
			=> r.Expand(-top, -left, -bottom, -right, keepCenter);
	}
}
