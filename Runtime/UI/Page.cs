using cmdwtf.UnityTools.Attributes;

using UnityEngine;

namespace cmdwtf.UnityTools.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class Page : MonoBehaviour
	{
		public bool IsShown { get; internal set; } = false;
		public bool IsHidden => !IsShown;
		public bool IsHiding { get; internal set; }

		public float Alpha => canvasGroup.alpha;

		[SerializeField, Autohook]
		private CanvasGroup canvasGroup;

		public virtual bool PageDisabled
			=> gameObject.activeInHierarchy == false ||
			   enabled == false;

		protected virtual void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0;
		}

		protected virtual void Start() { }
		protected virtual void Update() { }

		public virtual void OnPageShowing() { }
		public virtual void OnPageShown() { }
		public virtual void OnPageHiding() { }
		public virtual void OnPageHidden() { }

		public IPageDescription GetDescription()
			=> new PageDescription()
				{
					Name = name,
					Page = this,
					Button = null,
				};
	}
}
