using System;

using UnityEngine;

namespace cmdwtf.UnityTools.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class RangedAttribute : PropertyAttribute
	{
		public Rect Bounds { get; }
		public int GUIHeight { get; }

		public RangedAttribute(float xMin, float xMax, float yMin, float yMax, int height = 1)
		{
			Bounds = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
			GUIHeight = height;
		}

		public RangedAttribute(int height = 1)
			: this(0, 1, 0, 1, height)
		{ }

		public RangedAttribute(float xMax, float yMax, int height = 1)
			: this(0, xMax, 0, yMax, height)
		{ }

	}
}
