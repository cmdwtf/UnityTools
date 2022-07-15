using UnityEngine;
using System;

namespace cmdwtf.UnityTools.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class EnumFlagAttribute : PropertyAttribute { }
}
