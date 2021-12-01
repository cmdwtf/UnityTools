using System;

namespace cmdwtf.UnityTools.Attributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class AutohookSelfAttribute : AutohookAttribute
	{
		public AutohookSelfAttribute()
			: base(AutohookContext.Self) { }
		public AutohookSelfAttribute(AutohookTemporality temporality)
			: base(AutohookContext.Self, temporality) { }
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class AutohookParentAttribute : AutohookAttribute
	{
		public AutohookParentAttribute()
			: base(AutohookContext.Parent) { }
		public AutohookParentAttribute(AutohookTemporality temporality)
			: base(AutohookContext.Parent, temporality) { }
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class AutohookChildAttribute : AutohookAttribute
	{
		public AutohookChildAttribute()
			: base(AutohookContext.Child) { }
		public AutohookChildAttribute(AutohookTemporality temporality)
			: base(AutohookContext.Child, temporality) { }
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class AutohookSiblingAttribute : AutohookAttribute
	{
		public AutohookSiblingAttribute()
			: base(AutohookContext.Sibling) { }
		public AutohookSiblingAttribute(AutohookTemporality temporality)
			: base(AutohookContext.Sibling, temporality) { }
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class AutohookRootAttribute : AutohookAttribute
	{
		public AutohookRootAttribute()
			: base(AutohookContext.Root) { }
		public AutohookRootAttribute(AutohookTemporality temporality)
			: base(AutohookContext.Root, temporality) { }
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class AutohookPrefabRootAttribute : AutohookAttribute
	{
		public AutohookPrefabRootAttribute()
			: base(AutohookContext.PrefabRoot) { }
		public AutohookPrefabRootAttribute(AutohookTemporality temporality)
			: base(AutohookContext.PrefabRoot, temporality) { }
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class AutohookSceneAttribute : AutohookAttribute
	{
		public AutohookSceneAttribute()
			: base(AutohookContext.Scene) { }
		public AutohookSceneAttribute(AutohookTemporality temporality)
			: base(AutohookContext.Scene, temporality) { }
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class AutohookTargetAttribute : AutohookAttribute
	{
		public AutohookTargetAttribute(string target)
			: base(AutohookContext.Target)
		{
			Target = target;
		}

		public AutohookTargetAttribute(string target, AutohookTemporality temporality)
			: base(AutohookContext.Target, temporality)
		{
			Target = target;
		}
	}

	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class AutohookTaggedAttribute : AutohookAttribute
	{
		public AutohookTaggedAttribute(string tag)
			: base(AutohookContext.Tagged)
		{
			Target = tag;
		}

		public AutohookTaggedAttribute(string tag, AutohookTemporality temporality)
			: base(AutohookContext.Tagged, temporality)
		{
			Target = tag;
		}
	}
}
