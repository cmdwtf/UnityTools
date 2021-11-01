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
		public static Action<AutohookAttribute, FieldInfo, MonoBehaviour> SingleObjectClassifier;
	    public static Action<AutohookAttribute, FieldInfo, (MonoBehaviour, MonoBehaviour[])> MultipleObjectClassifier;

	    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InjectScriptReferences()
	    {
	        SingleObjectClassifier += (attr, field, behavior) =>
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
	        MultipleObjectClassifier += (attr, field, behaviorPair) =>
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

	    private static void Inject()
	    {
	        MonoBehaviour[] behaviors = Object.FindObjectsOfType<MonoBehaviour>();
	        foreach (MonoBehaviour behavior in behaviors)
	        {
	            Type behaviorType = behavior.GetType();
	            FieldInfo[] behaviorFields = behaviorType.GetFields(AutohookAttribute.BindingFlags);
				foreach (FieldInfo field in behaviorFields)
				{
					if (!field.IsDefined(typeof(AutohookAttribute), false))
					{
						continue;
					}

					AutohookAttribute attr = field.GetCustomAttribute<AutohookAttribute>();

					if (attr.Temporality != AutohookTemporality.RuntimeSceneLoad)
					{
						continue;
					}

					SingleObjectClassifier?.Invoke(attr, field, behavior);
					MultipleObjectClassifier?.Invoke(attr, field, (behavior, behaviors));
				}
	        }
	    }
	}
}
