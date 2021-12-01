using System;
using System.Reflection;

using UnityEngine;

using Object = UnityEngine.Object;

namespace cmdwtf.UnityTools.Attributes
{
	// based on https://github.com/merpheus-dev/GetComponentAttribute
	// MIT licensed.
	public static class AutohookInjection
	{
		private static Action<AutohookAttribute, FieldInfo, MonoBehaviour> _singleObjectClassifier;
		private static Action<AutohookAttribute, FieldInfo, (MonoBehaviour, MonoBehaviour[])> _multipleObjectClassifier;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InjectScriptReferences()
		{
			_singleObjectClassifier += (attr, field, behavior) =>
			{
				object value = attr.GetComponentFromContext(behavior, field.FieldType);
				if (value != null)
				{
					field.SetValue(behavior, value);
				}
			};
			Inject();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void InjectGameObjectReferences()
		{
			_multipleObjectClassifier += (attr, field, behaviorPair) =>
			{
				if (attr.Context != AutohookContext.Target ||
					attr.Context != AutohookContext.Tagged)
				{
					return;
				}

				GameObject target = attr.GetTargetGameObject();
				field.SetValue(behaviorPair.Item1,
							   target.GetComponent(
								   field.GetValue(behaviorPair.Item1).GetType()
								   )
							   );
			};
			Inject();
		}

		internal static void InjectRuntimeOnDemandReferences(MonoBehaviour target)
		{
			Type behaviorType = target.GetType();
			FieldInfo[] behaviorFields = behaviorType.GetFields(AutohookAttribute.BindingFlags);
			foreach (FieldInfo field in behaviorFields)
			{
				if (!Attribute.IsDefined(field, typeof(AutohookAttribute), false))
				{
					continue;
				}

				AutohookAttribute attr = field.GetCustomAttribute<AutohookAttribute>();

				if (attr.Temporality != AutohookTemporality.RuntimeOnDemand)
				{
					continue;
				}

				object value = attr.GetComponentFromContext(target, field.FieldType);

				if (value != null)
				{
					field.SetValue(target, value);
				}
			}
		}

#if UNITY_EDITOR

		public static Component InjectSerializedReference(AutohookAttribute attr, UnityEditor.SerializedProperty property)
		{
			Component component = attr.GetComponentFromContext(property);

			if (component != null)
			{
				if (property.objectReferenceValue == null)
				{
					property.objectReferenceValue = component;
				}
			}

			return component;
		}

#endif // UNITY_EDITOR

		private static void Inject()
		{
			MonoBehaviour[] behaviors = Object.FindObjectsOfType<MonoBehaviour>();
			foreach (MonoBehaviour behavior in behaviors)
			{
				Type behaviorType = behavior.GetType();
				FieldInfo[] behaviorFields = behaviorType.GetFields(AutohookAttribute.BindingFlags);
				foreach (FieldInfo field in behaviorFields)
				{
					if (!Attribute.IsDefined(field, typeof(AutohookAttribute), false))
					{
						continue;
					}

					AutohookAttribute attr = field.GetCustomAttribute<AutohookAttribute>();

					if (attr.Temporality != AutohookTemporality.RuntimeSceneLoad)
					{
						continue;
					}

					_singleObjectClassifier?.Invoke(attr, field, behavior);
					_multipleObjectClassifier?.Invoke(attr, field, (behavior, behaviors));
				}
			}
		}
	}
}
