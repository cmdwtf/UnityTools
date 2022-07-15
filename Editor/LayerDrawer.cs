using cmdwtf.UnityTools.Attributes;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	// via https://answers.unity.com/questions/609385/type-for-layer-selection.html
	[UnityEditor.CustomPropertyDrawer(typeof(LayerAttribute))]
	public class LayerDrawer : CustomPropertyDrawer
	{
		private const int MinLayerValue = 0;
		private const int MaxLayerValue = 31;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			int index = property.intValue;
			if (index > MaxLayerValue)
			{
				Debug.LogWarning($"{nameof(LayerDrawer)}: layer index is to high: '{index}', is set to {MaxLayerValue}");
				index = MaxLayerValue;
			}
			else if (index < MinLayerValue)
			{
				Debug.LogWarning($"{nameof(LayerDrawer)}: layer index is to low: '{index}', is set to {MinLayerValue}");
				index = MinLayerValue;
			}
			property.intValue = EditorGUI.LayerField(position, label, index);
			EditorGUI.EndProperty();
		}
	}
}
