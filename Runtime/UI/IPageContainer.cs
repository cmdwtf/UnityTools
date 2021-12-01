using System.Collections.Generic;

namespace cmdwtf.UnityTools.UI
{
	public interface IPageContainer
	{
		IPageDescription CurrentPage { get; }
		IReadOnlyCollection<IPageDescription> Pages { get; }
		void OpenFirstPage();
		void OpenPage(string pageName);
		void NextPage();
		void PreviousPage();
		void ShowCurrentPage();
		void HideCurrentPage();
		void ShowCurrentButton();
		void HideCurrentButton();
		IPageDescription AddNewPageDescription();
		void AddPage(IPageDescription page);
	}
}
