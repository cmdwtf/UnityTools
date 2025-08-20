using System.Collections;

using cmdwtf.UnityTools.Attributes;

using UnityEngine;

namespace cmdwtf.UnityTools.UI
{
	public class PageSwitchingEventArgs : System.EventArgs
	{
		public Page NextPage { get; private set; }
		public Page CurrentPage { get; private set; }
		public bool CancelSwitch { get; set; }

		public PageSwitchingEventArgs(Page nextPage, Page currentPage)
		{
			NextPage = nextPage;
			CurrentPage = currentPage;
		}
	}
}
