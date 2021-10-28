using UnityEngine;

namespace cmdwtf.UnityTools
{
	/// <summary>
	/// A set of tools for creating and manipulating <see cref="GameObject"/>s,
	/// that don't work as extension methods.
	/// </summary>
	public static class GameObjectTools
	{
		/// <summary>
		/// Creates a new <see cref="GameObject"/>, with a component
		/// of type <typeparamref name="T"/>. That component is the
		/// return value. The name of the game object can be set manually
		/// by passing <paramref name="name"/>, or if left null (default),
		/// the resulting game object will have the name of the component type added.
		/// </summary>
		/// <param name="name">The (optional) name to give the resulting <see cref="GameObject"/>.</param>
		/// <typeparam name="T">The <see cref="Component"/> type to add an instance of to the result.</typeparam>
		/// <returns>The new <typeparamref name="T"/> created on the new <see cref="GameObject"/>.</returns>
		public static T NewGameObjectWith<T>(string name = null) where T : Component
		{
			name ??= typeof(T).Name;
			var go = new GameObject(name, typeof(T));
			return go.GetComponent<T>();
		}
	}
}
