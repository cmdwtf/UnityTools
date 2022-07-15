using cmdwtf.UnityTools.Attributes;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	[UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyDrawer : CustomPropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.enabled = false;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;
		}
	}
}
