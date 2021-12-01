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
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class AutohookAttribute : PropertyAttribute
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

		public AutohookAttribute(AutohookContext context,
								 AutohookVisibility visibility
		) : this(context, visibility, AutohookTemporality.Editor, string.Empty)
		{ }

		public AutohookAttribute(AutohookContext context,
								 AutohookTemporality temporality
		) : this(context, AutohookVisibility.Default, temporality, string.Empty)
		{ }

		public AutohookAttribute(AutohookContext context,
								 AutohookVisibility visibility,
								 AutohookTemporality temporality
			) : this(context, visibility, temporality, string.Empty)
		{ }

		public AutohookAttribute(AutohookContext context,
								 AutohookVisibility visibility,
								 AutohookTemporality temporality,
								 string target
			)
        {
            Context = context;
            Visibility = visibility;
			Temporality = temporality;
			Target = target;
		}

		internal static void EnsureRuntimeHook(MonoBehaviour behavior)
		{
			if (Application.isEditor || behavior == null)
			{
				return;
			}

			AutohookInjection.InjectRuntimeOnDemandReferences(behavior);
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

			throw new Exception($"Unsupported {nameof(Context)}: {Context}");
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
				case AutohookContext.Sibling:
				{
					if (behavior.transform.parent is null)
					{
						return null;
					}

					foreach (Transform sibling in behavior.transform.parent)
					{
						if (sibling.gameObject == behavior.gameObject)
						{
							continue;
						}

						Component found = sibling.GetComponent(type);

						if (found != null)
						{
							return found;
						}
					}

					return null;
				}
				case AutohookContext.Root:
					return behavior.transform.root.GetComponent(type);
#if UNITY_EDITOR
				case AutohookContext.PrefabRoot:
					return PrefabUtility.GetOutermostPrefabInstanceRoot(behavior.transform).GetComponent(type);
#endif // UNITY_EDITOR
			}

			// try the stateless contexts
			return GetComponentFromContext(type);
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
					case AutohookContext.Sibling:
					{
						if (component.transform.parent is null)
						{
							return null;
						}

						foreach (Transform sibling in component.transform.parent)
						{
							if (sibling.gameObject == component.gameObject)
							{
								continue;
							}

							Component found = sibling.GetComponent(type);

							if (found != null)
							{
								return found;
							}
						}

						return null;
					}
					case AutohookContext.Root:
						return component.transform.root.GetComponent(type);
					case AutohookContext.PrefabRoot:
						return PrefabUtility.GetOutermostPrefabInstanceRoot(component.transform).GetComponent(type);
				}

				// try the stateless contexts
				return GetComponentFromContext(type);
			}

			throw new Exception($"{root.targetObject} is not a {nameof(Component)}.");
		}

		private static Type GetTypeFromProperty(SerializedProperty property)
		{
			// start with the actual serialized type,
			Type owningComponentType = property.serializedObject.targetObject.GetType();

			while (owningComponentType != null)
			{
				// find the field,
				FieldInfo fieldInfo = owningComponentType.GetField(property.propertyPath, BindingFlags);

				if (fieldInfo != null)
				{
					return fieldInfo.FieldType;
				}

				// if it didn't exist on this type, try the base type.
				owningComponentType = owningComponentType.BaseType;
			}

			return null;
		}

#endif // UNITY_EDITOR

	}
}
