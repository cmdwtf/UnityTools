using System;
using System.Collections.Generic;

using UnityEditor;

namespace cmdwtf.UnityTools.Editor
{
	/// <summary>
	/// Extensions for dealing with <see cref="SerializedObject"/>.
	/// </summary>
	public static class SerializedObjectExtensions
	{
		/// <summary>
		/// Gets all of the property names and display names on the serialized object, and optionally it's children.
		/// </summary>
		/// <param name="serializedObject">The object to get the names from.</param>
		/// <param name="enterChildren"><see langword="true"/> to recurse into sub-objects, otherwise <see langword="false" /></param>
		/// <returns>An enumerable of tuples with both the name and display name of the properties.</returns>
		public static IEnumerable<(string name, string displayName)> GetAllPropertyNames(this SerializedObject serializedObject, bool enterChildren = true)
		{
			SerializedProperty props = serializedObject.GetIterator();

			// must call the first next to begin iterating.
			props.Next(true);

			while (props.Next(true))
			{
				yield return (props.name, props.displayName);
			}
		}

		/// <summary>
		/// Attempts to get the property at the given path.
		/// </summary>
		/// <param name="serializedObject">The object to get the property value from.</param>
		/// <param name="propertyPath">The path of the property to get.</param>
		/// <param name="value">The output value.</param>
		/// <typeparam name="T">The type to cast the property to.</typeparam>
		/// <returns>The output value.</returns>
		/// <exception cref="MissingMemberException">The property is unable to be found.</exception>
		/// <exception cref="InvalidCastException">The property was unable to be cast to the requested type.</exception>
		public static T GetProperty<T>(this SerializedObject serializedObject, string propertyPath)
		{
			SerializedProperty property = serializedObject.FindProperty(propertyPath);

			if (property == null)
			{
				throw new MissingMemberException($"Unable to find property at path {propertyPath}");
			}

			return property.GetValue<T>();
		}

		/// <summary>
		/// Attempts to get the property value at the given path. This will first find the property,
		/// and then attempt to cast the property to the requested type.
		/// </summary>
		/// <param name="serializedObject">The object to get the property value from.</param>
		/// <param name="propertyPath">The path of the property to get.</param>
		/// <param name="value">The output value.</param>
		/// <typeparam name="T">The type to cast the property to.</typeparam>
		/// <returns><see langword="true"/> if successful, false if the property couldn't be found.</returns>
		/// <exception cref="System.InvalidCastException">If the property was found but not able to be cast to the requested type.</exception>
		public static bool TryGetProperty<T>(this SerializedObject serializedObject, string propertyPath, out T value)
		{
			value = default;

			SerializedProperty property = serializedObject.FindProperty(propertyPath);

			if (property == null)
			{
				return false;
			}

			value = property.GetValue<T>();

			return true;
		}

		/// <summary>
		/// Attempts to set the property at the given path to the given value. This will attempt
		/// to set the value by boxing the object and using the `targetObject` field of <see cref="SerializedProperty"/>.
		/// </summary>
		/// <param name="serializedObject">The object to find the property on.</param>
		/// <param name="propertyPath">The path of the property to find to set.</param>
		/// <param name="value">The value to set.</param>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <returns><see langword="false"/> if the property was unable to be found, otherwise <see langword="true"/></returns>
		public static bool TrySetProperty<T>(this SerializedObject serializedObject, string propertyPath, T value)
		{
			SerializedProperty property = serializedObject.FindProperty(propertyPath);

			if (property == null)
			{
				return false;
			}

			property.SetValue(value);

			return true;
		}
	}
}
