using cmdwtf.UnityTools.Attributes;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	// via https://answers.unity.com/questions/609385/type-for-layer-selection.html
	[CustomPropertyDrawer(typeof(LayerAttribute))]
	public class LayerAttributeEditor : PropertyDrawer
	{
		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(pos, label, prop);
			int index = prop.intValue;
			if (index > 31)
			{
				Debug.LogWarning($"{nameof(LayerAttributeEditor)}: layer index is to high '" + index + "', is set to 31.");
				index = 31;
			}
			else if (index < 0)
			{
				Debug.LogWarning($"{nameof(LayerAttributeEditor)}: layer index is to low '" + index + "', is set to 0");
				index = 0;
			}
			prop.intValue = EditorGUI.LayerField(pos, label, index);
			EditorGUI.EndProperty();
		}
	}
}
