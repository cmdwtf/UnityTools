using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	public class ValueRangeLimitEditPopup: CustomPopupWindowContent<ValueRangeLimitEditPopup>
	{
		private ValueRange _value;
		public ValueRange Value => _value;

		public ValueRangeLimitEditPopup(ValueRange value)
			: base("Range Limits")
		{
			_value = value;
		}

		protected override void ShouldDrawGUI(Rect rect)
		{
			_value.minimumLimit = EditorGUILayout.FloatField($"Minimum Value", _value.minimumLimit);
			_value.maximumLimit = EditorGUILayout.FloatField($"Maximum Value", _value.maximumLimit);

			if (_value.maximumLimit < _value.minimumLimit)
			{
				_value.maximumLimit = _value.minimumLimit;
			}
		}
	}
}
