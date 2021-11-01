using UnityEngine;

namespace cmdwtf.UnityTools.Attributes
{
	/// <summary>
	/// Context with which <see cref="AutohookAttribute"/> should
	/// look to find the component from.
	/// </summary>
	public enum AutohookContext
	{
		/// <summary>
		/// The <see cref="Component"/> should come from the current <see cref="GameObject"/>.
		/// </summary>
		Self = 0,
		/// <summary>
		/// The <see cref="Component"/> should come from a parent <see cref="GameObject"/>.
		/// </summary>
		Parent = 1,
		/// <summary>
		/// The <see cref="Component"/> should come from a child <see cref="GameObject"/>.
		/// </summary>
		Child = 2,
		/// <summary>
		/// The <see cref="Component"/> should come from the root <see cref="GameObject"/>.
		/// This uses <see cref="Transform.root"/> to determine the root object.
		/// </summary>
		Root = 3,
		/// <summary>
		/// The <see cref="Component"/> should come from the root <see cref="GameObject"/> in the current prefab.
		/// This uses <c>PrefabUtility.GetOutermostPrefabInstanceRoot</c> to get the component from.
		/// </summary>
		PrefabRoot = 4,
		/// <summary>
		/// The <see cref="Component"/> should come from any <see cref="GameObject"/> in the scene.
		/// This uses <see cref="Object.FindObjectOfType(System.Type)"/> to find the component.
		/// </summary>
		Scene = 5,
		/// <summary>
		/// The <see cref="Component"/> should come from a specific <see cref="GameObject"/> in the scene.
		/// This uses <see cref="GameObject.Find"/> to find the component.
		/// </summary>
		Target = 6,
		/// <summary>
		/// The <see cref="Component"/> should come from a <see cref="GameObject"/> with specific <see cref="GameObject.tag"/> in the scene.
		/// This uses <see cref="GameObject.FindWithTag"/> to find the component.
		/// </summary>
		Tagged = 7,
	}
}
