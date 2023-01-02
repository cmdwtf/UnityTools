using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	/// <summary>
	/// Utilities specific to the <see cref="EditorGUILayout"/> class.
	/// </summary>
	public static class EditorGUILayoutEx
	{
		private static Rect? _lastRect;

		/// <inheritdoc cref="GUILayoutUtility.GetLastRect"/>
		public static Rect GetLastRect()
			=> _lastRect ?? Rect.zero;

		/// <inheritdoc cref="DropdownButtonLabeled(string,string,UnityEngine.FocusType,UnityEngine.GUIStyle,UnityEngine.GUIStyle,UnityEngine.GUILayoutOption[])"/>
		public static bool DropdownButtonLabeled(string label,
												 string dropdownContent,
												 FocusType dropdownFocusType,
												 GUIStyle labelStyle = null,
												 GUIStyle dropdownStyle = null,
												 params GUILayoutOption[] layoutOptions
		)
			=> DropdownButtonLabeled(label.ToGUIContent(), dropdownContent.ToGUIContent(), dropdownFocusType, labelStyle, dropdownStyle, layoutOptions);

		/// <inheritdoc cref="DropdownButtonLabeled(string,string,UnityEngine.FocusType,UnityEngine.GUIStyle,UnityEngine.GUIStyle,UnityEngine.GUILayoutOption[])"/>
		public static bool DropdownButtonLabeled(string label,
												 string dropdownContent,
												 FocusType dropdownFocusType,
												 params GUILayoutOption[] layoutOptions
		)
			=> DropdownButtonLabeled(label.ToGUIContent(), dropdownContent.ToGUIContent(), dropdownFocusType, null, null, layoutOptions);

		/// <inheritdoc cref="DropdownButtonLabeled(string,string,UnityEngine.FocusType,UnityEngine.GUIStyle,UnityEngine.GUIStyle,UnityEngine.GUILayoutOption[])"/>
		public static bool DropdownButtonLabeled(string label,
												 GUIContent dropdownContent,
												 FocusType dropdownFocusType,
												 GUIStyle labelStyle = null,
												 GUIStyle dropdownStyle = null,
												 params GUILayoutOption[] layoutOptions
		)
			=> DropdownButtonLabeled(label.ToGUIContent(), dropdownContent, dropdownFocusType, labelStyle, dropdownStyle, layoutOptions);

		/// <inheritdoc cref="DropdownButtonLabeled(string,string,UnityEngine.FocusType,UnityEngine.GUIStyle,UnityEngine.GUIStyle,UnityEngine.GUILayoutOption[])"/>
		public static bool DropdownButtonLabeled(string label,
												 GUIContent dropdownContent,
												 FocusType dropdownFocusType,
												 params GUILayoutOption[] layoutOptions
		)
			=> DropdownButtonLabeled(label.ToGUIContent(), dropdownContent, dropdownFocusType, null, null, layoutOptions);

		/// <inheritdoc cref="DropdownButtonLabeled(string,string,UnityEngine.FocusType,UnityEngine.GUIStyle,UnityEngine.GUIStyle,UnityEngine.GUILayoutOption[])"/>
		public static bool DropdownButtonLabeled(GUIContent label,
												 string dropdownContent,
												 FocusType dropdownFocusType,
												 GUIStyle labelStyle = null,
												 GUIStyle dropdownStyle = null,
												 params GUILayoutOption[] layoutOptions
		)
			=> DropdownButtonLabeled(label, dropdownContent.ToGUIContent(), dropdownFocusType, labelStyle, dropdownStyle, layoutOptions);

		/// <inheritdoc cref="DropdownButtonLabeled(string,string,UnityEngine.FocusType,UnityEngine.GUIStyle,UnityEngine.GUIStyle,UnityEngine.GUILayoutOption[])"/>
		public static bool DropdownButtonLabeled(GUIContent label,
												 string dropdownContent,
												 FocusType dropdownFocusType,
												 params GUILayoutOption[] layoutOptions
		)
			=> DropdownButtonLabeled(label, dropdownContent.ToGUIContent(), dropdownFocusType, null, null, layoutOptions);

		/// <inheritdoc cref="DropdownButtonLabeled(string,string,UnityEngine.FocusType,UnityEngine.GUIStyle,UnityEngine.GUIStyle,UnityEngine.GUILayoutOption[])"/>
		public static bool DropdownButtonLabeled(GUIContent label,
												 GUIContent dropdownContent,
												 FocusType dropdownFocusType,
												 params GUILayoutOption[] layoutOptions)
			=> DropdownButtonLabeled(label, dropdownContent, dropdownFocusType, null, null, layoutOptions);

		/// <summary>
		/// Draws a dropdown button with a label.
		/// </summary>
		/// <param name="label">The label text.</param>
		/// <param name="dropdownContent">What content should be on the button.</param>
		/// <param name="dropdownFocusType">The type of focus the button should use.</param>
		/// <param name="labelStyle">The optional style to draw the label with.</param>
		/// <param name="dropdownStyle">The optional style to draw the button with.</param>
		/// <param name="layoutOptions">Optional layout options.</param>
		/// <returns><see langword="true"/>, if the button was clicked.</returns>
		public static bool DropdownButtonLabeled(GUIContent label,
												 GUIContent dropdownContent,
												 FocusType dropdownFocusType,
												 GUIStyle labelStyle = null,
												 GUIStyle dropdownStyle = null,
												 params GUILayoutOption[] layoutOptions
		)
		{
			// get a 'whole' rect for our control.
			Rect rect = EditorGUILayout.GetControlRect(layoutOptions);

			// duplicate the main rect, and shrink it
			// to the size for for the label.
			var labelRect = new Rect(rect) { width = EditorGUIUtility.labelWidth };

			// duplicate the main rect, and shrink it
			// to the remaining size for the dropdown button, and
			// scoot it over the width of the label.
			var dropdownRect = new Rect(rect);
			dropdownRect.width -= labelRect.width;
			dropdownRect.x += labelRect.width;

			// draw the label
			if (labelStyle != null)
			{
				EditorGUI.LabelField(labelRect, label, labelStyle);
			}
			else
			{
				EditorGUI.LabelField(labelRect, label);
			}

			// store the last rect for the user to know where the dropdown button was drawn.
			_lastRect = dropdownRect;

			// draw the dropdown, and return the user clicked value from it.
			return dropdownStyle != null
					   ? EditorGUI.DropdownButton(dropdownRect, dropdownContent, dropdownFocusType, dropdownStyle)
					   : EditorGUI.DropdownButton(dropdownRect, dropdownContent, dropdownFocusType);
		}

		public static void DropShadowLabel(GUIContent content, GUIStyle style = default)
		{
			Rect r = EditorGUILayout.GetControlRect();

			if (style == default)
			{
				EditorGUI.DropShadowLabel(r, content);
			}
			else
			{
				EditorGUI.DropShadowLabel(r, content, style);
			}

			_lastRect = r;
		}

		public static void DropShadowLabel(string text, GUIStyle style = default)
			=> DropShadowLabel(new GUIContent(text), style);

		public static bool InlineHamburgerMenuButton()
		{
			GUIStyle hamburgerStyle = new("PaneOptions")
			{
				padding = GUI.skin.button.padding,
				margin = GUI.skin.button.margin,
				alignment = TextAnchor.MiddleCenter,
				fixedHeight = 0,
				imagePosition = ImagePosition.ImageOnly,
				stretchHeight = true,
				stretchWidth = false,
			};

			// increase the margin just a smidge so it fits inline better.
			hamburgerStyle.margin.top++;
			hamburgerStyle.margin.bottom++;

			return GUILayout.Button(GUIContent.none, hamburgerStyle);
		}
	}
}
