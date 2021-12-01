using System.Collections;

using cmdwtf.UnityTools.Attributes;

using UnityEngine;

namespace cmdwtf.UnityTools.UI
{
	public class PageManager<TPageContainer> : MonoBehaviour where TPageContainer : Component, IPageContainer
	{
		[SerializeField, Autohook]
		private TPageContainer pages;

		private void Reset()
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

		private void Awake()
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
		}

		private void Start()
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

			Page current = GetActivePage();

			current.OnPageHiding();
			nextPage.OnPageShowing();

			pages.OpenPage(nextPage.name);

			StartCoroutine(HidingCoroutine(current));

			nextPage.IsShown = true;
			nextPage.OnPageShown();

			IEnumerator HidingCoroutine(Page page)
			{
				current.IsHiding = true;

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

		private Page GetActivePage() => pages.CurrentPage.Page;
	}
}
