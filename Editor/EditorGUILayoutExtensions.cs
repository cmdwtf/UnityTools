using cmdwtf.UnityTools;

using UnityEditor;

using UnityEngine;

public static class EditorGUILayoutExtensions
{
	/// <inheritdoc cref="EditorGUI.DropdownButton(UnityEngine.Rect,UnityEngine.GUIContent,UnityEngine.FocusType)"/>
	public static bool DropdownButtonLabeled(this EditorGUILayout egl,
											 string label,
											 string dropdownContent,
											 FocusType dropdownFocusType,
											 GUIStyle labelStyle = null,
											 GUIStyle dropdownStyle = null
	)
		=> DropdownButtonLabeled(egl, label.ToGUIContent(), dropdownContent.ToGUIContent(), dropdownFocusType, labelStyle, dropdownStyle);

	/// <inheritdoc cref="EditorGUI.DropdownButton(UnityEngine.Rect,UnityEngine.GUIContent,UnityEngine.FocusType)"/>
	public static bool DropdownButtonLabeled(this EditorGUILayout egl,
											 string label,
											 GUIContent dropdownContent,
											 FocusType dropdownFocusType,
											 GUIStyle labelStyle = null,
											 GUIStyle dropdownStyle = null
	)
		=> DropdownButtonLabeled(egl, label.ToGUIContent(), dropdownContent, dropdownFocusType, labelStyle, dropdownStyle);

	/// <inheritdoc cref="EditorGUI.DropdownButton(UnityEngine.Rect,UnityEngine.GUIContent,UnityEngine.FocusType)"/>
	public static bool DropdownButtonLabeled(this EditorGUILayout egl,
											 GUIContent label,
											 string dropdownContent,
											 FocusType dropdownFocusType,
											 GUIStyle labelStyle = null,
											 GUIStyle dropdownStyle = null
	)
		=> DropdownButtonLabeled(egl, label, dropdownContent.ToGUIContent(), dropdownFocusType, labelStyle, dropdownStyle);

	/// <inheritdoc cref="EditorGUI.DropdownButton(UnityEngine.Rect,UnityEngine.GUIContent,UnityEngine.FocusType)"/>
	public static bool DropdownButtonLabeled(this EditorGUILayout egl,
											 GUIContent label,
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

		// draw the dropdown, and return the user clicked value from it.
		return dropdownStyle != null
				   ? EditorGUI.DropdownButton(dropdownRect, dropdownContent, dropdownFocusType, dropdownStyle)
				   : EditorGUI.DropdownButton(dropdownRect, dropdownContent, dropdownFocusType);
	}
}
