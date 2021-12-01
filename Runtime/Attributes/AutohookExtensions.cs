using UnityEngine;

namespace cmdwtf.UnityTools.Attributes
{
	public static class AutohookExtensions
	{
		public static void EnsureRuntimeAutohooks(this MonoBehaviour b)
			=> AutohookInjection.InjectRuntimeOnDemandReferences(b);
	}
}
