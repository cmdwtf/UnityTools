using System;

using UnityEngine;

namespace cmdwtf.UnityTools.Attributes
{
	/// <summary>
	/// Attribute that require implementation of the provided interface.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class RequireInterfaceAttribute : PropertyAttribute
	{
		// Interface type.
		public System.Type requiredType { get; private set; }

		/// <summary>
		/// Requiring implementation of the <see cref="T:Tools.RequireInterfaceAttribute"/> interface.
		/// </summary>
		/// <param name="type">Interface type.</param>
		public RequireInterfaceAttribute(System.Type type)
		{
			requiredType = type;
		}
	}
}
