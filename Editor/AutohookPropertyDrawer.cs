using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BrunoMikoski.Framework.AutoHook
{
    [CustomPropertyDrawer(typeof(AutohookAttribute))]
    public sealed class AutohookPropertyDrawer : PropertyDrawer
    {
        private Visibility visibility;

        private const BindingFlags BINDIN_FLAGS = BindingFlags.IgnoreCase
                                                  | BindingFlags.Public
                                                  | BindingFlags.Instance
                                                  | BindingFlags.NonPublic;

        private AutohookAttribute AutoHookAttribute { get { return (AutohookAttribute)attribute; } }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Component component = FindAutohookTarget(property);
            if (component != null)
            {
                if (property.objectReferenceValue == null)
                    property.objectReferenceValue = component;

                if (visibility == Visibility.Hidden)
                    return;
            }

            bool guiEnabled = GUI.enabled;
            if (visibility == Visibility.Disabled && component != null)
                GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = guiEnabled;
        }

        private void UpdateVisibility()
        {
            visibility = AutoHookAttribute.Visibility;
            if (visibility == Visibility.Default)
            {
                visibility = (Visibility)EditorPrefs.GetInt(AutoHookEditorSettings.AUTO_HOOK_VISIBILITY_KEY,
                    0);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            UpdateVisibility();

            Component component = FindAutohookTarget(property);
            if (component != null && visibility == Visibility.Hidden)
                return 0;

            return base.GetPropertyHeight(property, label);
        }

        private Component FindAutohookTarget(SerializedProperty property)
        {
            SerializedObject root = property.serializedObject;

            if (root.targetObject is Component)
            {
                Type type = GetTypeFromProperty(property);

                Component component = (Component)root.targetObject;
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
                throw new Exception(root.targetObject + "is not a component");
            }

            return null;
        }

        private static Type GetTypeFromProperty(SerializedProperty property)
        {
            Type parentComponentType = property.serializedObject.targetObject.GetType();
            FieldInfo fieldInfo = parentComponentType.GetField(property.propertyPath, BINDIN_FLAGS);
            return fieldInfo.FieldType;
        }
    }
}
