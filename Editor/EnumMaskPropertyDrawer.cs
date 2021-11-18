using cmdwtf.UnityTools.Attributes;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	[CustomPropertyDrawer(typeof(EnumMaskAttribute))]
	public class EnumMaskAttributePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginChangeCheck();

			int enumValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);

			if (EditorGUI.EndChangeCheck()) {
				property.intValue = enumValue;
			}
		}
	}
}
