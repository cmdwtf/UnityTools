using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
    [CustomPropertyDrawer(typeof(AutohookAttribute))]
    internal sealed class AutohookPropertyDrawer : PropertyDrawer
    {
        private Visibility _visibility;

        private const BindingFlags BindingFlags = System.Reflection.BindingFlags.IgnoreCase
                                                  | System.Reflection.BindingFlags.Public
                                                  | System.Reflection.BindingFlags.Instance
                                                  | System.Reflection.BindingFlags.NonPublic;

        private AutohookAttribute AutoHookAttribute => (AutohookAttribute)attribute;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Component component = FindAutohookTarget(property);
            if (component != null)
            {
                if (property.objectReferenceValue == null)
				{
					property.objectReferenceValue = component;
				}

				if (_visibility == Visibility.Hidden)
				{
					return;
				}
			}

            bool guiEnabled = GUI.enabled;
            if (_visibility == Visibility.Disabled && component != null)
			{
				GUI.enabled = false;
			}

			EditorGUI.PropertyField(position, property, label);
            GUI.enabled = guiEnabled;
        }

        private void UpdateVisibility()
		{
            _visibility = AutoHookAttribute.Visibility;

            if (_visibility == Visibility.Default)
			{
				var settings = AutohookSettings.GetOrCreateSettings();
				_visibility = settings.defaultVisibility;
			}
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            UpdateVisibility();

            Component component = FindAutohookTarget(property);
            if (component != null && _visibility == Visibility.Hidden)
			{
				return 0;
			}

			return base.GetPropertyHeight(property, label);
        }

        private Component FindAutohookTarget(SerializedProperty property)
        {
            SerializedObject root = property.serializedObject;

            if (root.targetObject is Component component)
            {
                Type type = GetTypeFromProperty(property);

				switch (AutoHookAttribute.Context)
                {
                    case Context.Self:
                        return component.GetComponent(type);
                    case Context.Child:
                    {
                        Component[] options = component.GetComponentsInChildren(type, true);
                        return options.Length > 0 ? options[0] : null;
                    }
                    case Context.Parent:
                    {
                        Component[] options = component.GetComponentsInParent(type, true);
                        return options.Length > 0 ? options[0] : null;
                    }
                    case Context.Root:
                    {
                        return component.transform.root.GetComponent(type);
                    }
                    case Context.PrefabRoot:
                    {
                        return PrefabUtility.GetOutermostPrefabInstanceRoot(component.transform).GetComponent(type);
                    }
                }
            }
            else
            {
                throw new Exception($"{root.targetObject} is not a {nameof(Component)}.");
            }

            return null;
        }

        private static Type GetTypeFromProperty(SerializedProperty property)
        {
            Type parentComponentType = property.serializedObject.targetObject.GetType();
            FieldInfo fieldInfo = parentComponentType.GetField(property.propertyPath, BindingFlags);
            return fieldInfo?.FieldType;
        }
    }
}
