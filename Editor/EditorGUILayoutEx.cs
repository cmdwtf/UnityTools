using cmdwtf.UnityTools;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	public static class EditorGUILayoutEx
	{
		private static Rect? _lastRect = null;
		
		/// <inheritdoc cref="EditorGUI.DropdownButton(UnityEngine.Rect,UnityEngine.GUIContent,UnityEngine.FocusType)"/>
		public static bool DropdownButtonLabeled(string label,
												 string dropdownContent,
												 FocusType dropdownFocusType,
												 GUIStyle labelStyle = null,
												 GUIStyle dropdownStyle = null
		)
			=> DropdownButtonLabeled(label.ToGUIContent(), dropdownContent.ToGUIContent(), dropdownFocusType, labelStyle, dropdownStyle);

		/// <inheritdoc cref="EditorGUI.DropdownButton(UnityEngine.Rect,UnityEngine.GUIContent,UnityEngine.FocusType)"/>
		public static bool DropdownButtonLabeled(string label,
												 GUIContent dropdownContent,
												 FocusType dropdownFocusType,
												 GUIStyle labelStyle = null,
												 GUIStyle dropdownStyle = null
		)
			=> DropdownButtonLabeled(label.ToGUIContent(), dropdownContent, dropdownFocusType, labelStyle, dropdownStyle);

		/// <inheritdoc cref="EditorGUI.DropdownButton(UnityEngine.Rect,UnityEngine.GUIContent,UnityEngine.FocusType)"/>
		public static bool DropdownButtonLabeled(GUIContent label,
												 string dropdownContent,
												 FocusType dropdownFocusType,
												 GUIStyle labelStyle = null,
												 GUIStyle dropdownStyle = null
		)
			=> DropdownButtonLabeled(label, dropdownContent.ToGUIContent(), dropdownFocusType, labelStyle, dropdownStyle);

		/// <inheritdoc cref="EditorGUI.DropdownButton(UnityEngine.Rect,UnityEngine.GUIContent,UnityEngine.FocusType)"/>
		public static bool DropdownButtonLabeled(GUIContent label,
												 GUIContent dropdownContent,
												 FocusType dropdownFocusType,
												 GUIStyle labelStyle = null,
												 GUIStyle dropdownStyle = null
		)
		{
			const float fortyPercent = 0.4f;
			const float sixtyPercent = 0.6f;

			// get a 'whole' rect for our control.
			Rect rect = EditorGUILayout.GetControlRect();

			// duplicate the main rect, and shrink it
			// to 40% for the label.
			var labelRect = new Rect(rect);
			labelRect.width *= fortyPercent;

			// duplicate the main rect, and shrink it
			// to 60% for the dropdown button, and
			// scoot it over the width of the label.
			var dropdownRect = new Rect(rect);
			dropdownRect.width *= sixtyPercent;
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

		/// <inheritdoc cref="GUILayoutUtility.GetLastRect"/>
		public static Rect GetLastRect()
			=> _lastRect ?? Rect.zero;
	}
}