using UnityEditor;
using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	// based on https://gist.github.com/aarthificial/f2dbb58e4dbafd0a93713a380b9612af

#if UNITY_2020_1_OR_NEWER
	[UnityEditor.CustomPropertyDrawer(typeof(Optional<>))]
	public class OptionalPropertyDrawer : CustomPropertyDrawer
	{
		private const string ValuePropertyName = nameof(Optional<object>.value);
		private const string EnabledPropertyName = nameof(Optional<object>.enabled);
		private float _toggleWidthPixels = 0f;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty valueProperty = property.FindPropertyRelative(ValuePropertyName);
			SerializedProperty enabledProperty = property.FindPropertyRelative(EnabledPropertyName);

			if (_toggleWidthPixels == 0)
			{
				// hoping that the toggle width is the same as the height 🤷‍♀️
				_toggleWidthPixels = EditorGUI.GetPropertyHeight(enabledProperty);
			}

			EditorGUI.BeginProperty(position, label, property);
			position.width -= _toggleWidthPixels + (Constants.StandardHorizontalSpacing / 2);
			EditorGUI.BeginDisabledGroup(!enabledProperty.boolValue);
			EditorGUI.PropertyField(position, valueProperty, label, true);
			EditorGUI.EndDisabledGroup();

			int originalIndent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			position.x += position.width + Constants.StandardHorizontalSpacing;
			position.width = position.height = _toggleWidthPixels;
			EditorGUI.PropertyField(position, enabledProperty, GUIContent.none);
			EditorGUI.indentLevel = originalIndent;
			EditorGUI.EndProperty();
		}
	}
#endif // UNITY_2020_1_OR_NEWER
}
