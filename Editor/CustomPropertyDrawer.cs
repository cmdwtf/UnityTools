using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

using Random = UnityEngine.Random;

namespace cmdwtf.UnityTools.Editor
{
	/// <summary>
	/// A base class for custom property drawers.
	/// Provides common functionality such as calculating simple height based on line count,
	/// and the ability to easily 'delegate' a primary property to a child property.
	/// Note that this name potentially collides with
	/// a similarly named (but sealed) class in the UnityEditor namespace.
	/// </summary>
	public abstract class CustomPropertyDrawer : PropertyDrawer
	{
		private static readonly HashSet<string> EmptySet = new();

		protected virtual int PropertyLineCount { get; set; } = 1;

		protected virtual int GetPropertyLineCount(SerializedProperty property, GUIContent label) => PropertyLineCount;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			PropertyLineCount = GetPropertyLineCount(GetDelegatedSubProperty(property), label);
			float lineHeights = EditorGUIUtility.singleLineHeight * PropertyLineCount;
			float paddingHeights = EditorGUIUtility.standardVerticalSpacing * (MathF.Max(0, PropertyLineCount - 1));
			return lineHeights + paddingHeights;
		}

		protected T GetCustomAttribute<T>() where T : PropertyAttribute => Attribute.GetCustomAttribute(fieldInfo, typeof(T)) as T;

		protected string DelegatedSubPropertyName { get; set; }

		protected SerializedProperty GetDelegatedSubProperty(SerializedProperty root)
			=> string.IsNullOrEmpty(DelegatedSubPropertyName)
				? root
				: root.FindPropertyRelative(DelegatedSubPropertyName);

		protected float GetDelegatedPropertyHeight(SerializedProperty property, GUIContent label, bool includeChildren = true)
		{
			SerializedProperty delegateProperty = GetDelegatedSubProperty(property);

			if (delegateProperty == null)
			{
				throw new MissingMemberException($"Unable to find delegated sub property: {DelegatedSubPropertyName}");
			}

			return EditorGUI.GetPropertyHeight(delegateProperty, label, includeChildren);
		}

		protected bool DelegatedPropertyField(ref SerializedProperty property,
											  GUIContent label,
											  bool includeChildren = true
		)
		{
			float height = GetDelegatedPropertyHeight(property, label, includeChildren);
			Rect rect = EditorGUILayout.GetControlRect(label != null, height);
			return DelegatedPropertyField(rect, ref property, label, includeChildren);
		}

		protected bool DelegatedPropertyField(Rect position, ref SerializedProperty property, GUIContent label, bool includeChildren = true)
		{
			// a helper to do a property field, and report if children were hidden, and return if there was a change.
			bool DoProperty(ref SerializedProperty p)
			{
				EditorGUI.BeginChangeCheck();

				bool hiddenChildren = EditorGUI.PropertyField(position, p, label, includeChildren);

				if (hiddenChildren)
				{
					Debug.LogWarning($"{nameof(DelegatedPropertyField)}: a property field was rendered that has " +
					                 $"children that want to draw, but they were not drawn, due to {nameof(includeChildren)}=false.");
				}

				return EditorGUI.EndChangeCheck();
			}

			// get the 'real' property
			SerializedProperty delegateProperty = GetDelegatedSubProperty(property);

			// our 'real' property is just the normal one, draw and be done.
			if (delegateProperty == property)
			{
				return DoProperty(ref property);
			}

			// propagate our parent's expansion state to our delegate
			delegateProperty.isExpanded = property.isExpanded;

			// draw the field(s)
			bool wasChanged = DoProperty(ref delegateProperty);

			// re-assign our parent's expansion property based on the expansion of our (potentially changed) delegate's expansion
			property.isExpanded = delegateProperty.isExpanded;

			// go ahead and store our changes if our property was modified!
			if (wasChanged)
			{
				property.serializedObject.ApplyModifiedProperties();
			}

			return wasChanged;
		}

		protected void ChildrenPropertyFieldsLayout(SerializedProperty property,
													HashSet<string> skipProperties
		)
		{
			float height = GetChildPropertyFieldsHeight(property, skipProperties);
			Rect position = EditorGUILayout.GetControlRect(hasLabel: false, height);
			OnGUIChildren(position, property, skipProperties);
		}

		protected void OnGUIChildren(Rect position, SerializedProperty property)
			=> OnGUIChildren(position, property, EmptySet);

		protected void OnGUIChildren(Rect position, SerializedProperty property, HashSet<string> skipProperties)
		{
			void DrawProperty(SerializedProperty childProp)
			{
				GUIContent content = new(childProp.displayName, childProp.tooltip);
				position.height = EditorGUI.GetPropertyHeight(childProp, content, childProp.isExpanded);
				EditorGUI.PropertyField(position, childProp, content, childProp.isExpanded);

				position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
			}

			ForEachChildProperty(property, DrawProperty, skipProperties);
		}

		private void ForEachChildProperty(SerializedProperty property, Action<SerializedProperty> action, HashSet<string> skipProperties)
		{
			bool firstChild = true;

			// don't mess with the caller's copy, make our own.
			SerializedProperty iteratorCopy = property.Copy();

			// know where we need to end.
			SerializedProperty endChildren = property.GetEndProperty();

			// go through the visible properties, skipping ones we were asked to skip.
			// the first step needs to be 'true' to step into the child, otherwise we
			// will jump the whole property!
			while (iteratorCopy.NextVisible(firstChild) &&
				   !SerializedProperty.EqualContents(iteratorCopy, endChildren))
			{
				firstChild = false;

				// skip properties we aren't interested in displaying
				if (skipProperties.Contains(iteratorCopy.name))
				{
					continue;
				}

				action.Invoke(iteratorCopy);
			}
		}

		protected float GetChildPropertyFieldsHeight(SerializedProperty property)
			=> GetChildPropertyFieldsHeight(property, EmptySet);

		protected float GetChildPropertyFieldsHeight(SerializedProperty property, HashSet<string> skipProperties)
		{
			float padding = EditorGUIUtility.standardVerticalSpacing;
			float totalHeight = 0f;
			ForEachChildProperty(property, GetHeight, skipProperties);

			// each child will add it's intra-control padding, but we need to subtract the last one
			// because that padding wouldn't count into the total of *our* height
			return MathF.Max(0, totalHeight - padding);

			void GetHeight(SerializedProperty childProp)
			{
				float height = EditorGUI.GetPropertyHeight(childProp, GUIContent.none, childProp.isExpanded);
				totalHeight += height + padding;
			}
		}
	}
}
