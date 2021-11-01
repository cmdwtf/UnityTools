using System;
using System.Reflection;

using UnityEditor;

using UnityEngine;

using Object = UnityEngine.Object;

namespace cmdwtf.UnityTools.Attributes
{
	/// <summary>
	/// An attribute that will cause the <see cref="Component"/> it's decorating
	/// to automatically find a reference to hook up to.
	/// The <see cref="Context"/> can be configured to look for components
	/// outside of this <see cref="GameObject"/>.
	/// The <see cref="Visibility"/> can be set to control how the field is
	/// drawn in the inspector.
	/// </summary>
	public sealed class AutohookAttribute : PropertyAttribute
    {
		// binding flags used to reflect on types.
		public const BindingFlags BindingFlags = System.Reflection.BindingFlags.IgnoreCase
												  | System.Reflection.BindingFlags.Public
												  | System.Reflection.BindingFlags.Instance
												  | System.Reflection.BindingFlags.NonPublic;

		public AutohookContext Context { get; set; }
		public AutohookVisibility Visibility { get; set; }
		public AutohookTemporality Temporality { get; set; }
		public string Target { get; set; }

		public AutohookAttribute()
			: this(AutohookContext.Self, AutohookVisibility.Default, AutohookTemporality.Editor, string.Empty)
        { }

        public AutohookAttribute(AutohookContext context)
			: this()
        {
            Context = context;
		}

		public AutohookAttribute(AutohookVisibility visibility)
			: this()
		{
			Visibility = visibility;
		}

		public AutohookAttribute(AutohookTemporality temporality)
			: this()
		{
			Temporality = temporality;
		}

        public AutohookAttribute(AutohookContext context, AutohookVisibility visibility, AutohookTemporality temporality, string target)
        {
            Context = context;
            Visibility = visibility;
			Temporality = temporality;
			Target = target;
		}

		public GameObject GetTargetGameObject()
		{
			switch (Context)
			{
				case AutohookContext.Target:
					return GameObject.Find(Target);
				case AutohookContext.Tagged:
					return GameObject.FindWithTag(Target);
			}

			return null;
		}

		public Component GetComponentFromContext(Type type)
		{
			switch (Context)
			{
				case AutohookContext.Scene:
					return Object.FindObjectOfType(type) as Component;
				case AutohookContext.Target:
				case AutohookContext.Tagged:
					return GetTargetGameObject().GetComponent(type);
			}

			return null;
		}

		public Component GetComponentFromContext(MonoBehaviour behavior, Type type)
		{
			switch (Context)
			{
				case AutohookContext.Self:
					return behavior.GetComponent(type);
				case AutohookContext.Child:
				{
					Component[] options = behavior.GetComponentsInChildren(type, true);
					return options.Length > 0 ? options[0] : null;
				}
				case AutohookContext.Parent:
				{
					Component[] options = behavior.GetComponentsInParent(type, true);
					return options.Length > 0 ? options[0] : null;
				}
				case AutohookContext.Root:
					return behavior.transform.root.GetComponent(type);
#if UNITY_EDITOR
				case AutohookContext.PrefabRoot:
					return PrefabUtility.GetOutermostPrefabInstanceRoot(behavior.transform).GetComponent(type);
#endif // UNITY_EDITOR
			}

			// try the stateless contexts, or throw an exception.
			return GetComponentFromContext(type)
				   ?? throw new Exception($"Unsupported {nameof(Context)}: {Context}");
		}

#if UNITY_EDITOR

		public Component GetComponentFromContext(SerializedProperty property)
		{
			SerializedObject root = property.serializedObject;
			Type type = GetTypeFromProperty(property);

			if (root.targetObject is Component component)
			{
				switch (Context)
				{
					case AutohookContext.Self:
						return component.GetComponent(type);
					case AutohookContext.Child:
					{
						Component[] options = component.GetComponentsInChildren(type, true);
						return options.Length > 0 ? options[0] : null;
					}
					case AutohookContext.Parent:
					{
						Component[] options = component.GetComponentsInParent(type, true);
						return options.Length > 0 ? options[0] : null;
					}
					case AutohookContext.Root:
						return component.transform.root.GetComponent(type);
					case AutohookContext.PrefabRoot:
						return PrefabUtility.GetOutermostPrefabInstanceRoot(component.transform).GetComponent(type);
				}

				// try the stateless contexts, or throw an exception.
				return GetComponentFromContext(type)
					   ?? throw new Exception($"Unsupported {nameof(Context)}: {Context}");
			}

			throw new Exception($"{root.targetObject} is not a {nameof(Component)}.");
		}

		private static Type GetTypeFromProperty(SerializedProperty property)
		{
			Type parentComponentType = property.serializedObject.targetObject.GetType();
			FieldInfo fieldInfo = parentComponentType.GetField(property.propertyPath, BindingFlags);
			return fieldInfo?.FieldType;
		}

#endif // UNITY_EDITOR

	}
}
