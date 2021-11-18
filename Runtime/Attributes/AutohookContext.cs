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
		/// This uses <see cref="GameObject.GetComponent{T}"/> to find the component.
		/// </summary>
		Self,
		/// <summary>
		/// The <see cref="Component"/> should come from a parent <see cref="GameObject"/>.
		/// This uses <see cref="GameObject.GetComponentInParent{T}()"/> to find the component.
		/// </summary>
		Parent,
		/// <summary>
		/// The <see cref="Component"/> should come from a this or a child <see cref="GameObject"/>.
		/// This uses <see cref="GameObject.GetComponentInChildren{T}()"/> to find the component.
		/// </summary>
		Child,
		/// <summary>
		/// The <see cref="Component"/> should come from a sibling <see cref="GameObject"/>.
		/// This uses <see cref="GameObject.GetComponent{T}"/> on each of the objects that
		/// share the same parent, skipping this game object.
		/// </summary>
		Sibling,
		/// <summary>
		/// The <see cref="Component"/> should come from the root <see cref="GameObject"/>.
		/// This uses <see cref="Transform.root"/> to determine the root object.
		/// </summary>
		Root,
#if UNITY_EDITOR
		/// <summary>
		/// The <see cref="Component"/> should come from the root <see cref="GameObject"/> in the current prefab.
		/// This uses <see cref="PrefabUtility.GetOutermostPrefabInstanceRoot"/> to get the component from.
		/// </summary>
#endif // UNITY_EDITOR
		PrefabRoot,
		/// <summary>
		/// The <see cref="Component"/> should come from any <see cref="GameObject"/> in the scene.
		/// This uses <see cref="Object.FindObjectOfType(System.Type)"/> to find the component.
		/// </summary>
		Scene,
		/// <summary>
		/// The <see cref="Component"/> should come from a specific <see cref="GameObject"/> in the scene.
		/// This uses <see cref="GameObject.Find"/> to find the component.
		/// </summary>
		Target,
		/// <summary>
		/// The <see cref="Component"/> should come from a <see cref="GameObject"/> with specific <see cref="GameObject.tag"/> in the scene.
		/// This uses <see cref="GameObject.FindWithTag"/> to find the component.
		/// </summary>
		Tagged,
	}
}
