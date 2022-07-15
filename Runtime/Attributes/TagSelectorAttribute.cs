using System;

using UnityEngine;

namespace cmdwtf.UnityTools.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class TagSelectorAttribute : PropertyAttribute
	{
		public bool UseDefaultTagFieldDrawer = false;
	}
}
