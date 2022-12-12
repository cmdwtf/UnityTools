using cmdwtf.UnityTools.Attributes;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	[UnityEditor.CustomPropertyDrawer(typeof(RangedAttribute))]
	public class RangedDrawer : CustomPropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (attribute is not RangedAttribute boundedCurve)
			{
				return EditorGUIUtility.singleLineHeight;
			}

			return EditorGUIUtility.singleLineHeight * boundedCurve.GUIHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (attribute is not RangedAttribute boundedCurve)
			{
				EditorGUI.HelpBox(position, $"Invalid attribute type. Expected {nameof(RangedAttribute)}.", MessageType.Error);
				return;
			}

			EditorGUI.BeginProperty(position, label, property);
			property.animationCurveValue = EditorGUI.CurveField(
				position,
				label,
				property.animationCurveValue,
				Color.white,
				boundedCurve.Bounds
			);
			EditorGUI.EndProperty();
		}
	}
}
