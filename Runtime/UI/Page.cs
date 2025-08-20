using cmdwtf.UnityTools.Attributes;

using UnityEngine;
using UnityEngine.Events;

namespace cmdwtf.UnityTools.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class Page : MonoBehaviour
	{
		public bool IsShown { get; internal set; } = false;
		public bool IsHidden => !IsShown;
		public bool IsHiding { get; internal set; }

		public float Alpha => canvasGroup.alpha;

		[SerializeField]
		internal PageLayer[] underlays;

		[SerializeField]
		internal PageLayer[] overlays;

		[SerializeField, Autohook]
		private CanvasGroup canvasGroup;

		[SerializeField]
		private UnityEvent pageShowing;
		[SerializeField]
		private UnityEvent pageShown;
		[SerializeField]
		private UnityEvent pageHiding;
		[SerializeField]
		private UnityEvent pageHidden;

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

		public virtual void OnPageShowing()
		{
			pageShowing?.Invoke();
		}

		public virtual void OnPageShown()
		{
			pageShown?.Invoke();
		}

		public virtual void OnPageHiding()
		{
			pageHiding?.Invoke();
		}

		public virtual void OnPageHidden()
		{
			pageHidden?.Invoke();
		}

		public IPageDescription GetDescription()
			=> new PageDescription()
			{
				Name = name,
				Page = this,
				Button = null,
			};
	}
}
