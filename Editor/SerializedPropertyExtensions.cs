using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	/// <summary>
	/// Extensions for dealing with <see cref="SerializedProperty"/>.
	/// Some code via: https://gist.github.com/aholkner/214628a05b15f0bb169660945ac7923b
	/// </summary>
	public static class SerializedPropertyExtensions
	{
		private static readonly Regex ArrayElementRegex = new(@"\GArray\.data\[(\d+)\]", RegexOptions.Compiled);

		public static uint GetContentHash(this SerializedProperty property)
		{
#if UNITY_2022_1_OR_NEWER
			return property.contentHash;
#else
			return (uint)property.GetValue().GetHashCode();
#endif // UNITY_2022_1_OR_NEWER
		}

		/// (Extension) Get the value of the serialized property.
		public static object GetValue(this SerializedProperty property)
		{
			string propertyPath = property.propertyPath;
			object value = property.serializedObject.targetObject;
			int i = 0;
			while (NextPathComponent(propertyPath, ref i, out PropertyPathComponent token))
			{
				value = GetPathComponentValue(value, token);
			}

			return value;
		}

		/// (Extension) Get the value of the serialized property.
		public static T GetValue<T>(this SerializedProperty property)
		{
			object objValue = property.GetValue();
			if (objValue is T value)
			{
				return value;
			}

			throw new InvalidCastException($"Couldn't convert {property.name} to {typeof(T).FullName}");
		}

		/// (Extension) Set the value of the serialized property.
		public static void SetValue(this SerializedProperty property, object value)
		{
			Undo.RecordObject(property.serializedObject.targetObject, $"Set {property.displayName}");
			property.SetValueNoUndoRecord(value);
		}

		/// (Extension) Set the value of the serialized property without recording an undo state.
		public static void SetValueNoUndoRecord(this SerializedProperty property, object value)
		{
			SetValueNoRecord(property, value);

			EditorUtility.SetDirty(property.serializedObject.targetObject);
			property.serializedObject.ApplyModifiedProperties();
		}


#if UNITY_2022_1_OR_NEWER
		/// (Extension) Set the value of the serialized property, but do not record the change.
		/// The change will not be persisted unless you call SetDirty and ApplyModifiedProperties.
		public static void SetValueNoRecord(this SerializedProperty property, object value)
		{
			property.boxedValue = value;
		}

#else

		/// (Extension) Set the value of the serialized property, but do not record the change.
		/// The change will not be persisted unless you call SetDirty and ApplyModifiedProperties.
		public static void SetValueNoRecord(this SerializedProperty property, object value)
		{
			string propertyPath = property.propertyPath;
			object container = property.serializedObject.targetObject;

			int i = 0;
			NextPathComponent(propertyPath, ref i, out PropertyPathComponent deferredToken);
			while (NextPathComponent(propertyPath, ref i, out PropertyPathComponent token))
			{
				container = GetPathComponentValue(container, deferredToken);
				deferredToken = token;
			}

			Debug.Assert(!container.GetType().IsValueType,
						 $"Cannot use SerializedObject.SetValue on a struct object, as the result will be set on a temporary.  Either change {container.GetType().Name} to a class, or use SetValue with a parent member.");
			SetPathComponentValue(container, deferredToken, value);
		}

#endif // UNITY_2022_1_OR_NEWER

		// Union type representing either a property name or array element index.  The element
		// index is valid only if propertyName is null.
		private struct PropertyPathComponent
		{
			public string PropertyName;
			public int ElementIndex;
		}

		/// <summary>
		/// Gets the names of all properties on the given property.
		/// </summary>
		/// <param name="property">The property go get the names from.</param>
		/// <returns>The names from the properties.</returns>
		public static IEnumerable<string> GetAllPropertyNames(this SerializedProperty property)
		{
			IEnumerator props = property.GetEnumerator();

			while (props.MoveNext())
			{
				yield return (props.Current as SerializedProperty)?.name;
			}
		}

		// Parse the next path component from a SerializedProperty.propertyPath.  For simple field/property access,
		// this is just tokenizing on '.' and returning each field/property name.  Array/list access is via
		// the pseudo-property "Array.data[N]", so this method parses that and returns just the array/list index N.
		//
		// Call this method repeatedly to access all path components.  For example:
		//
		//      string propertyPath = "quests.Array.data[0].goal";
		//      int i = 0;
		//      NextPropertyPathToken(propertyPath, ref i, out var component);
		//          => component = { propertyName = "quests" };
		//      NextPropertyPathToken(propertyPath, ref i, out var component)
		//          => component = { elementIndex = 0 };
		//      NextPropertyPathToken(propertyPath, ref i, out var component)
		//          => component = { propertyName = "goal" };
		//      NextPropertyPathToken(propertyPath, ref i, out var component)
		//          => returns false
		private static bool NextPathComponent(string propertyPath, ref int index, out PropertyPathComponent component)
		{
			component = new PropertyPathComponent();

			if (index >= propertyPath.Length)
			{
				return false;
			}

			Match arrayElementMatch = ArrayElementRegex.Match(propertyPath, index);
			if (arrayElementMatch.Success)
			{
				index += arrayElementMatch.Length + 1; // Skip past next '.'
				component.ElementIndex = int.Parse(arrayElementMatch.Groups[1].Value);
				return true;
			}

			int dot = propertyPath.IndexOf('.', index);
			if (dot == -1)
			{
				component.PropertyName = propertyPath.Substring(index);
				index = propertyPath.Length;
			}
			else
			{
				component.PropertyName = propertyPath.Substring(index, dot - index);
				index = dot + 1; // Skip past next '.'
			}

			return true;
		}

		private static object GetPathComponentValue(object container, PropertyPathComponent component)
			=> component.PropertyName == null
				   ? ((IList)container)[component.ElementIndex]
				   : GetMemberValue(container, component.PropertyName);

		private static void SetPathComponentValue(object container, PropertyPathComponent component, object value)
		{
			if (component.PropertyName == null)
			{
				((IList)container)[component.ElementIndex] = value;
			}
			else
			{
				SetMemberValue(container, component.PropertyName, value);
			}
		}

		private static object GetMemberValue(object container, string name)
		{
			if (container == null)
			{
				return null;
			}

			Type type = container.GetType();
			MemberInfo[] members =
				type.GetMember(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			foreach (MemberInfo t in members)
			{
				switch (t)
				{
					case FieldInfo field:
						return field.GetValue(container);
					case PropertyInfo property:
						return property.GetValue(container);
				}
			}

			return null;
		}

		private static void SetMemberValue(object container, string name, object value)
		{
			Type type = container.GetType();
			MemberInfo[] members =
				type.GetMember(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			foreach (MemberInfo t in members)
			{
				switch (t)
				{
					case FieldInfo field:
						field.SetValue(container, value);
						return;
					case PropertyInfo property:
						property.SetValue(container, value);
						return;
				}
			}

			Debug.Assert(false, $"Failed to set member {container}.{name} via reflection");
		}
	}
}
