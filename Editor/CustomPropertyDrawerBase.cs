using cmdwtf.UnityTools.Attributes;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	public class CustomPropertyDrawerBase : PropertyDrawer
	{
		protected int PropertyLineCount { get; set; } = 1;

		protected virtual int GetPropertyLineCount(SerializedProperty property, GUIContent label) => 1;
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			PropertyLineCount = GetPropertyLineCount(property, label);
			float lineHeights = EditorGUIUtility.singleLineHeight * PropertyLineCount;
			float paddingHeights = EditorGUIUtility.standardVerticalSpacing * PropertyLineCount;
			return lineHeights + paddingHeights;
		} 
	}
}
