using UnityEngine;

namespace cmdwtf.UnityTools.Editor.RipoffParticleUI
{
	/// <summary>
	/// An interface representing a property container that is a simple property.
	/// </summary>
	public interface ISimplePropertyContainer : IPropertyContainer
	{
		/// <summary>
		/// Adds a simple property to this container.
		/// </summary>
		/// <param name="propertyName">The property to add.</param>
		/// <param name="label">The GUI content associated with the property.</param>
		/// <returns>The representation of the property.</returns>
		SimpleProperty AddSimpleProperty(string propertyName, GUIContent label = null);

		/// <summary>
		/// Adds a simple property to this container.
		/// </summary>
		/// <param name="propertyName">The property to add.</param>
		/// <param name="label">The GUI content associated with the property.</param>
		/// <returns>The representation of the property.</returns>
		SimpleProperty AddSimpleProperty(string propertyName, string label);

		/// <summary>
		/// Removes the property from this container.
		/// </summary>
		/// <param name="propertyName">The property to remove.</param>
		void RemoveSimpleProperty(string propertyName);

		/// <summary>
		/// Sets the visibility of the given property.
		/// </summary>
		/// <param name="propertyName">The property to set visibility on.</param>
		/// <param name="isVisible"><see langword="true"/>, if the property should be visible.</param>
		void SetSimplePropertyVisible(string propertyName, bool isVisible);
	}
}
