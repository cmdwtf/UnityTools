using UnityEngine;

namespace cmdwtf.UnityTools
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
		public Context Context { get; }
		public Visibility Visibility { get; }

		public AutohookAttribute()
			: this(UnityTools.Context.Self, Visibility.Default)
        { }

        public AutohookAttribute(Context context)
			: this()
        {
            Context = context;
		}

        public AutohookAttribute(Visibility visibility)
			: this()
        {
            Visibility = visibility;
        }

        public AutohookAttribute(Context context, Visibility visibility)
        {
            Context = context;
            Visibility = visibility;
        }
    }
}
