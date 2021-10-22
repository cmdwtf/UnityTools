using UnityEngine;

namespace cmdwtf.UnityTools.Attributes
{
	public class MinMaxAttribute : PropertyAttribute
	{
		public readonly float MinLimit;
		public readonly float MaxLimit;
		public bool ShowEditRange = false;
		public bool ShowDebugValues = false;

		public MinMaxAttribute(int min, int max)
		{
			MinLimit = min;
			MaxLimit = max;
		}
	}
}