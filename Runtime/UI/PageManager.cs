using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using cmdwtf.UnityTools.Attributes;

using UnityEngine;

namespace cmdwtf.UnityTools.UI
{
	public class PageManager<TPageContainer> : MonoBehaviour where TPageContainer : Component, IPageContainer
	{
		[SerializeField, Autohook]
		private TPageContainer pages;

		private GameObject _underlayParent;
		private GameObject _overlayParent;
		private PageLayer[] _layers;

		public event EventHandler<PageSwitchingEventArgs> PageSwitching;

		protected virtual void Reset()
		{
			var existing = GetComponent<TPageContainer>();
			if (existing != null)
			{
				pages = existing;
				return;
			}

			var newContainer = gameObject.AddComponent<TPageContainer>();
			pages = newContainer;
		}

		protected virtual void Awake()
		{
			if (pages == null)
			{
				Debug.LogError($"{name} expects an {nameof(IPageContainer)} to manage it's pages internally");
				Destroy(this);
				return;
			}

			// load up the page names with the pages in the window manager.
			Page[] childPages = GetComponentsInChildren<Page>();

			foreach (Page page in childPages)
			{
				// skip disabled pages
				if (page.PageDisabled)
				{
					continue;
				}

				pages.AddPage(page.GetDescription());
			}

			SetupLayers(childPages);
		}

		protected virtual void Start()
		{
			// set our initial page
			pages.OpenFirstPage();
		}

		public void SwitchToPage(Page nextPage)
		{
			if (nextPage.IsShown && !nextPage.IsHiding)
			{
				Debug.LogWarning($"Not switching to {nextPage.name}, page is already shown.");
				return;
			}

			Page currentPage = GetActivePage();

			PageSwitchingEventArgs args = new(nextPage, currentPage);

			PageSwitching?.Invoke(this, args);

			if (args.CancelSwitch)
			{
				Debug.Log("Page switch cancelled by event.");
				return;
			}

			foreach (PageLayer l in _layers)
			{
				l.OnPageSwitching(args);
			}

			currentPage.OnPageHiding();
			nextPage.OnPageShowing();

			pages.OpenPage(nextPage.name);

			StartCoroutine(HidingCoroutine(currentPage));

			nextPage.IsShown = true;
			nextPage.OnPageShown();

			IEnumerator HidingCoroutine(Page page)
			{
				currentPage.IsHiding = true;

				// don't mark the page hidden
				// until it's completely faded out.
				while (page.Alpha > 0)
				{
					yield return null;
				}

				page.IsHiding = false;
				page.IsShown = false;
				page.OnPageHidden();
			}
		}

		private void SetupLayers(IEnumerable<Page> pages)
		{
			// find or create underlay parent
			PageUnderlayParent ulp = GetComponentInChildren<PageUnderlayParent>();

			if (ulp != null)
			{
				_underlayParent = ulp.gameObject;
			}
			else
			{
				_underlayParent = new GameObject($"{gameObject.name} — Underlays",
					typeof(RectTransform), typeof(PageUnderlayParent));
			}

			RectTransform ulpt = (RectTransform)_underlayParent.transform;

			ulpt.SetParentAndStretchUI(transform);
			ulpt.SetAsFirstSibling();

			// find or create overlay parent
			PageOverlayParent olp = GetComponentInChildren<PageOverlayParent>();

			if (olp != null)
			{
				_overlayParent = olp.gameObject;
			}
			else
			{
				_overlayParent = new GameObject($"{gameObject.name} — Overlays",
					typeof(RectTransform), typeof(PageOverlayParent));
			}

			RectTransform olpt = (RectTransform)_overlayParent.transform;

			olpt.SetParentAndStretchUI(transform);
			olpt.SetAsLastSibling();

			// move all the page layers to their correct parents,
			// and let them know what pages they belong to.
			_layers = GetComponentsInChildren<PageLayer>();

			foreach (PageLayer layer in _layers)
			{
				Transform layerParent = layer.Type == PageLayerType.Overlay
					? _overlayParent.transform
					: _underlayParent.transform;

				layer.transform.SetParent(layerParent, worldPositionStays: false);

				layer.TargetPages = layer.IsGlobalLayer
						? pages.ToArray()
						: pages.Where(
								p => p.overlays.Contains(layer) || p.underlays.Contains(layer)
							).ToArray();
			}
		}

		private Page GetActivePage() => pages.CurrentPage.Page;
	}
}
