using UnityEngine;

using UnityEditor;

namespace cmdwtf.UnityTools.Editor
{
	[CustomPropertyDrawer(typeof(ValueRange))]
	public class ValueRangePropertyDrawer : CustomPropertyDrawerBase
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var val = property.GetValue<ValueRange>();
			
			float min = val.minimum;
			float max = val.maximum;
			float minLimit = val.minimumLimit;
			float maxLimit = val.maximumLimit;
			
			EditorGUI.MinMaxSlider(position.EditorGUILineHeight(), label, ref min, ref max, minLimit, maxLimit);

			if (max < min)
			{
				max = min;
			}
			
			position.EditorGUINextLine();
			EditorGUI.LabelField(position.EditorGUILineHeight(), " ", val.ToFullString());

			position.EditorGUINextLine();
			val.canEditLimit = EditorGUI.Toggle(position.EditorGUILineHeight(), "  Can Edit Limit", val.canEditLimit);
			
			if (val.canEditLimit)
			{
				position.EditorGUINextLine();
				minLimit = EditorGUI.FloatField(position.EditorGUILineHeight(), $"    Minimum", minLimit);
				position.EditorGUINextLine();
				maxLimit = EditorGUI.FloatField(position.EditorGUILineHeight(), $"    Maximum", maxLimit);

				if (maxLimit < minLimit)
				{
					maxLimit = minLimit;
				}
			}

			val.minimum = min;
			val.maximum = max;
			val.minimumLimit = minLimit;
			val.maximumLimit = maxLimit;
			
			//property.SetTarget(val);
			//property.SetValue(val);
			property.SetValue(val);
		}

		protected override int GetPropertyLineCount(SerializedProperty property, GUIContent label)
		{
			var val = property.GetValue<ValueRange>();
			return 3 + (val.canEditLimit ? 2 : 0);
		}
	}
}
