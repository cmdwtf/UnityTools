using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	/// <summary>
	/// Property drawer for SerializableGuid
	///
	/// Based on https://forum.unity.com/threads/cannot-serialize-a-guid-field-in-class.156862/#post-6996680 by Searous
	/// </summary>
	[CustomPropertyDrawer(typeof(SerializableGuid))]
	public class SerializableGuidPropertyDrawer : PropertyDrawer
	{
		public static bool AllowEditing { get; set; } = true;
		private static int ControlLines => AllowEditing ? 2 : 1;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			float lineHeight = EditorGUIUtility.singleLineHeight;
			float spacer = EditorGUIUtility.standardVerticalSpacing;
			float horizontalSpacer = spacer * 2;

			// Start property draw
			using var scope = new EditorGUI.PropertyScope(position, label, property);
			label = scope.content;

			// Get properties
			SerializedProperty serializedGuid = property.FindPropertyRelative(nameof(SerializableGuid.serializedGuid));

			GUI.enabled = AllowEditing;

			position.height -= spacer * (ControlLines - 1);

			// Draw label centered
			if (ControlLines > 1)
			{
				position = position.EditorGUICenterVertical();
			}

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			// Bump the other controls back off the 'centered' line position.
			if (ControlLines > 1)
			{
				position.y -= lineHeight / 2f;
			}

			// and give us some breathing room.
			position.height -= spacer;

			if (AllowEditing)
			{
				Rect buttonRect = position.EditorGUILineHeightTabs(3, horizontalSpacer);

				// Buttons
				if (GUI.Button(buttonRect, "New"))
				{
					serializedGuid.stringValue = System.Guid.NewGuid().ToString();
				}

				buttonRect.EditorGUINextTab(horizontalSpacer);

				if (GUI.Button(buttonRect, "Copy"))
				{
					EditorGUIUtility.systemCopyBuffer = serializedGuid.stringValue;
				}

				buttonRect.EditorGUINextTab(horizontalSpacer);

				if (GUI.Button(buttonRect, "Empty"))
				{
					serializedGuid.stringValue = System.Guid.Empty.ToString();
				}

				position.EditorGUINextLine();
			}

			EditorGUI.PropertyField(position, serializedGuid, GUIContent.none);

			GUI.enabled = true;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float controlHeight = EditorGUIUtility.singleLineHeight * ControlLines;
			return controlHeight + (EditorGUIUtility.standardVerticalSpacing * ControlLines);
		}
	}
}
