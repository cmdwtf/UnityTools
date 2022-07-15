using System;
using System.Drawing;
using System.Reflection;

using UnityEngine;

using Color = UnityEngine.Color;

namespace cmdwtf.UnityTools.Attributes
{
	// ReSharper disable once InconsistentNaming
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class DynamicsLineColorAttribute : PropertyAttribute
	{
		public Color color { get; private set; }

		public DynamicsLineColorAttribute(KnownColor knownColor)
		{
			var sdColor = System.Drawing.Color.FromKnownColor(knownColor);
			const float byteMax = byte.MaxValue;
			color = new Color(
				sdColor.R / byteMax,
				sdColor.G / byteMax,
				sdColor.B / byteMax,
				sdColor.A / byteMax
			);
		}
	}
}
