using UnityEngine;

using UnityEditor;

namespace cmdwtf.UnityTools.Editor
{
	[UnityEditor.CustomPropertyDrawer(typeof(ValueRange))]
	public class ValueRangeDrawer : CustomPropertyDrawer
	{
		private Rect _limitPopupRect;
		private ValueRange? _popupEditedValue;

		private readonly GUIStyle _textStyle = new(EditorStyles.label);
		private readonly GUIStyle _numberStyle = new(EditorStyles.numberField);

		private readonly GUIContent _upIcon = EditorGUIUtility.IconContent("d_ProfilerTimelineRollUpArrow");
		private readonly GUIContent _dnIcon = EditorGUIUtility.IconContent("d_ProfilerTimelineDigDownArrow");

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var value = property.GetValue<ValueRange>();

			GUI.enabled = property.editable;

			EditorGUI.BeginChangeCheck();

			if (value.forceFixedValue)
			{
				if (property.isExpanded)
				{
					property.isExpanded = false;
				}

				DrawSimpleSlider(position, property, label, ref value);
			}
			else
			{
				DrawMinMaxSlider(position, property, label, ref value);
			}

			if (EditorGUI.EndChangeCheck() && GUI.enabled)
			{
				UpdatePropertyFromValue(position, label, property, ref value);
			}

			GUI.enabled = true;
		}

		private void UpdatePropertyFromValue(Rect position, GUIContent label, SerializedProperty property, ref ValueRange value)
		{
			EditorGUI.BeginProperty(position, label, property);
			SerializedProperty range = property.FindPropertyRelative(nameof(ValueRange.range));
			SerializedProperty limit = property.FindPropertyRelative(nameof(ValueRange.limit));
			SerializedProperty ffv = property.FindPropertyRelative(nameof(ValueRange._forceFixedValue));

			range.vector2Value = value.range;
			limit.vector2Value = value.limit;
			ffv.boolValue = value._forceFixedValue;
			EditorGUI.EndProperty();
		}

		private void DrawSimpleSlider(Rect position, SerializedProperty property, GUIContent label, ref ValueRange value)
		{
			value.fixedValue = EditorGUI.Slider(position, label, value.minimum, value.minimumLimit, value.maximumLimit);
		}

		private void DrawMinMaxSlider(Rect position, SerializedProperty property, GUIContent label, ref ValueRange value)
		{
			float min = value.minimum;
			float max = value.maximum;

			GUI.enabled = GUI.enabled && value.isFixedValue == false;

			GUIContent minValueContent = new() { tooltip = "Minimum Value" };
			GUIContent maxValueContent = new() { tooltip = "Maximum Value" };
			float twoSpaces = Constants.StandardHorizontalSpacing * 2f;

			property.isExpanded = EditorGUI.Foldout(position.EditorGUILineHeight(), property.isExpanded, label);
			using (EditorGUI.IndentLevelScope indent = new(-EditorGUI.indentLevel))
			{
				Rect valSliderPos = position.EditorGUIFieldWidth();
				GUIContent valSliderContent = new() {tooltip = $"{min} - {max}"};

				// if we're collapsed, shrink the slider and draw it between two
				// shortened float fields of the min and max.
				if (!property.isExpanded)
				{
					float fixedFieldWidth = valSliderPos.width * 0.20f;

					float newMin = EditorGUI.FloatField(valSliderPos.CollapseToLeft(fixedFieldWidth), minValueContent, min, _numberStyle);
					float newMax = EditorGUI.FloatField(valSliderPos.CollapseToRight(fixedFieldWidth), maxValueContent, max, _numberStyle);
					valSliderPos= valSliderPos.Shrink((fixedFieldWidth+twoSpaces) * 2f, 0);

					min = (newMin > max) ? max : newMin;
					max = (newMax < min) ? min : newMax;
				}

				// draw the slider
				EditorGUI.MinMaxSlider(valSliderPos, valSliderContent, ref min, ref max, value.minimumLimit,
					value.maximumLimit);
			}

			if (property.isExpanded)
			{
				using EditorGUI.IndentLevelScope indent = new(1);
				position.EditorGUINextLine();

				Rect labelRect = position.EditorGUIFieldWidth();

				// for some reason, the label looks about 1 pixel off.
				labelRect.y--;

				// draw minimum limit
				GUI.Label(labelRect, $"[{value.minimumLimit}...", _textStyle.Left());

				// draw maximum limit
				GUI.Label(labelRect, $"...{value.maximumLimit}]", _textStyle.Right());

				float widthScale = 0.3f;
				Rect minFieldRect = labelRect.WidthScale(widthScale);
				Rect maxFieldRect = labelRect.WidthScale(widthScale);
				float halfWidthAndSpace = (minFieldRect.width / 2f) + Constants.StandardHorizontalSpacing;

				minFieldRect = minFieldRect.BumpLeft(halfWidthAndSpace);
				maxFieldRect = maxFieldRect.BumpRight(halfWidthAndSpace);

				GUI.Label(minFieldRect, _dnIcon, _textStyle.Left());
				GUI.Label(maxFieldRect, _upIcon, _textStyle.Left());

				float newMin = EditorGUI.FloatField(minFieldRect, GUIContent.none, min);
				float newMax = EditorGUI.FloatField(maxFieldRect, GUIContent.none, max);

				min = (newMin > max) ? max : newMin;
				max = (newMax < min) ? min : newMax;

				GUI.enabled = property.editable;

				GUIContent limitsButtonContent = new("Limits");
				_limitPopupRect = position.EditorGUILabelWidth().IndentedRect(1);
				_limitPopupRect.width -= Constants.StandardHorizontalSpacing;

				if (EditorGUI.DropdownButton(_limitPopupRect, limitsButtonContent, FocusType.Keyboard, EditorStyles.miniPullDown.Left()))
				{
					ValueRangeLimitEditPopup popup = new(value) {Width = 250f};
					popup.AcceptClicked += p => _popupEditedValue = p.Value;

					PopupWindow.Show(_limitPopupRect, popup);
				}
			}

			// store our updated min/max
			value.minimum = min;
			value.maximum = max;

			// did the user edit the limits in the popup?
			// if not, we're done.
			if (!_popupEditedValue.HasValue)
			{
				return;
			}

			// if they did, update the new limits and mark the UI as changed!
			value.minimumLimit = _popupEditedValue.Value.minimumLimit;
			value.maximumLimit = _popupEditedValue.Value.maximumLimit;
			_popupEditedValue = null;
			GUI.changed = true;
		}

		protected override int GetPropertyLineCount(SerializedProperty property, GUIContent label)
		{
			PropertyLineCount = 1 + (property.isExpanded ? 1 : 0);
			return PropertyLineCount;
		}
	}
}
