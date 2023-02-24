using System;

namespace cmdwtf.UnityTools
{
	/// <summary>
	/// An attribute that can be applied to a <see cref="UniqueScriptableObject{T}"/> to adjust the sub-Resources
	/// name of the folder the asset is saved in.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class UniqueNamespaceAttribute : Attribute
	{
		public string Namespace { get; }

		public UniqueNamespaceAttribute(string @namespace)
		{
			Namespace = @namespace;
		}
	}
}
