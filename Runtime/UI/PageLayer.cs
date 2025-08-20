#region

using System;
using System.Linq;

using cmdwtf.UnityTools.Attributes;

using UnityEngine;

#endregion

namespace cmdwtf.UnityTools.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class PageLayer : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("The type of layer (whether it should be under or over the active page.)")]
		private PageLayerType type = PageLayerType.Overlay;

		[SerializeField]
		[Tooltip("If a layer is global, it will be shown on all pages, except pages specified in its ignores.")]
		private bool globalLayer = false;

		[SerializeField]
		[Tooltip("An optional list of pages for this layer to ignore. This will disable this layer on those pages, even if the layer is global.")]
		private Page[] ignores;

		[SerializeField, Autohook]
		private CanvasGroup canvasGroup;

		public PageLayerType Type => type;

		public bool IsGlobalLayer => globalLayer;

		private Page[] _targetPages = Array.Empty<Page>();
		internal Page[] TargetPages
		{
			get => _targetPages;
			set
			{
				_targetPages = value == null
					? Array.Empty<Page>()
					: value.Where(p => !ignores.Contains(p)).ToArray();
			}
		}

		public float Alpha
		{
			get => canvasGroup.alpha;
			private set => canvasGroup.alpha = value;
		}

		private Page _currentShownPage;

		private enum State
		{
			Hidden,
			Showing,
			Shown,
			Hiding
		}

		private State _state = State.Hidden;

		private void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();

			// make sure we don't have an animator,
			// as that will mess with our manually adjusting alpha.

			Animator animator = GetComponent<Animator>();

			if (animator != null)
			{
				Debug.LogWarning($"{gameObject.name} has a {nameof(PageLayer)} and animator component.\n" +
					$"These components conflict, and the animator has been removed. Remove it manually " +
					$"to stop seeing this warning in the future.");
				Destroy(animator);
			}
		}

		private void LateUpdate()
		{
			float targetAlpha = 0.0f;

			switch (_state)
			{
				case State.Hidden:
					targetAlpha = 0.0f;
					break;
				case State.Showing:
					// if we are showing, set our alpha to the largest of
					// the page that is showing us, or our current alpha
					// (that is in the case where we switch pages where both
					// pages show us, we don't want to fade out at all in those cases.)
					targetAlpha = Mathf.Max(_currentShownPage.Alpha, Alpha);
					if (Mathf.Approximately(targetAlpha, 1.0f))
					{
						_state = State.Shown;
					}
					break;
				case State.Shown:
					targetAlpha = 1.0f;
					break;
				case State.Hiding:
					targetAlpha = _currentShownPage.Alpha;
					if (targetAlpha == 0)
					{
						_currentShownPage = null;
						_state = State.Hidden;
					}
					break;
				default:
					break;
			}

			Alpha = targetAlpha;
		}

		internal void OnPageSwitching(PageSwitchingEventArgs e)
		{
			bool nextPageIsTarget = IsTargetPage(e.NextPage);

			// if we aren't hidden and the next page is not a target,
			// we will start hiding.
			if (_state != State.Hidden && nextPageIsTarget == false)
			{
				_state = State.Hiding;
				return;
			}

			// if we were hidden and the next page isn't a target, we're done.
			if (!nextPageIsTarget)
			{
				return;
			}

			// switch to showing with the next page as the page
			// that is showing us.
			_state = State.Showing;
			_currentShownPage = e.NextPage;
		}

		private bool IsTargetPage(Page page) => TargetPages.Contains(page);
	}
}
