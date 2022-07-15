using cmdwtf.UnityTools.Attributes;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	// via http://www.sharkbombs.com/2015/02/17/unity-editor-enum-flags-as-toggle-buttons/
	[UnityEditor.CustomPropertyDrawer(typeof(EnumFlagAttribute))]
	public class EnumFlagsDrawer : CustomPropertyDrawer
	{
		private static readonly GUIStyle ToggleGuiStyle = GUI.skin.button;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			int buttonsIntValue = 0;
			int enumLength = property.enumNames.Length;
			bool[] buttonPressed = new bool[enumLength];
			float buttonWidth = (position.width - EditorGUIUtility.labelWidth) / enumLength;

			EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height), label);

			EditorGUI.BeginChangeCheck();

			for (int i = 0; i < enumLength; i++) {

				// Check if the button is/was pressed
				if ( ( property.intValue & (1 << i) ) == 1 << i ) {
					buttonPressed[i] = true;
				}

				var buttonPos = new Rect(position.x + EditorGUIUtility.labelWidth + (buttonWidth * i), position.y, buttonWidth, position.height);

				buttonPressed[i] = GUI.Toggle(buttonPos, buttonPressed[i], property.enumNames[i],  ToggleGuiStyle);

				if (buttonPressed[i])
				{
					buttonsIntValue += 1 << i;
				}
			}

			if (EditorGUI.EndChangeCheck()) {
				property.intValue = buttonsIntValue;
			}
		}
	}

}
