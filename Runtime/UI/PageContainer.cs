using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace cmdwtf.UnityTools.UI
{
	public class PageContainer : MonoBehaviour, IPageContainer
	{
		public IPageDescription CurrentPage
		{
			get
			{
				if (_selectedPage < 0 || _selectedPage >= _pages.Count)
				{
					return null;
				}

				return _pages[_selectedPage];
			}
		}

		public IReadOnlyCollection<IPageDescription> Pages { get; }

		private readonly List<IPageDescription> _pages = new List<IPageDescription>();
		private int _selectedPage = -1;

		public PageContainer()
		{
			Pages = _pages.AsReadOnly();
		}

		public bool OpenPage(int index)
		{
			if (index < 0 || index >= _pages.Count)
			{
				Debug.LogWarning($"Page index {index} out of range, won't open page.");
				return false;
			}

			_selectedPage = index;
			ShowCurrentPage();
			ShowCurrentButton();
			return true;
		}

		public void OpenFirstPage()
		{
			if (_pages.Any())
			{
				OpenPage(0);
			}
		}

		public void OpenPage(string pageName)
		{
			int targetIndex = _pages.FindIndex(p => p.Name == pageName);

			if (targetIndex < 0)
			{
				Debug.LogWarning($"Failed to find page {pageName}, won't open.");
				return;
			}

			OpenPage(targetIndex);
		}

		public void NextPage() => OpenPage(_selectedPage + 1);

		public void PreviousPage() => OpenPage(_selectedPage - 1);

		public void ShowCurrentPage()
		{
			if (CurrentPage?.Page != null)
			{
				CurrentPage.Page.gameObject.SetActive(true);
			}
		}

		public void HideCurrentPage()
		{
			if (CurrentPage?.Page != null)
			{
				CurrentPage.Page.gameObject.SetActive(false);
			}
		}

		public void ShowCurrentButton()
		{
			if (CurrentPage?.Button != null)
			{
				CurrentPage.Button.SetActive(true);
			}
		}

		public void HideCurrentButton()
		{
			if (CurrentPage?.Button != null)
			{
				CurrentPage.Button.SetActive(false);
			}
		}

		public IPageDescription AddNewPageDescription()
		{
			IPageDescription desc = new PageDescription();
			_pages.Add(desc);
			return desc;
		}

		public void AddPage(IPageDescription newPage) => _pages.Add(newPage);
	}
}
