using System.Collections.Generic;

using UnityEditor;

namespace cmdwtf.UnityTools.Editor
{
	/// <summary>
	/// Utilities specific to the <see cref="EditorGUIUtility"/> class.
	/// </summary>
	public static class EditorGUIUtilityEx
	{
		private static readonly Stack<float> _labelWidths = new();
		private static readonly Stack<float> _fieldWidths = new();

		public static void PushLabelWidth(float width)
		{
			_labelWidths.Push(EditorGUIUtility.labelWidth);
			EditorGUIUtility.labelWidth = width;
		}

		public static bool PopLabelWidth()
		{
			if (!_labelWidths.TryPop(out float width))
			{
				return false;
			}

			EditorGUIUtility.labelWidth = width;
			return true;

		}

		public static void PushFieldWidth(float width)
		{
			_fieldWidths.Push(EditorGUIUtility.fieldWidth);
			EditorGUIUtility.fieldWidth = width;
		}

		public static bool PopFieldWidth()
		{
			if (!_fieldWidths.TryPop(out float width))
			{
				return false;
			}

			EditorGUIUtility.fieldWidth = width;
			return true;

		}
	}
}
