using UnityEngine;

namespace cmdwtf.UnityTools.UI
{
	public class PageDescription : IPageDescription
	{
		public string Name { get; set; }
		public Page Page { get; set; }
		public GameObject Button { get; set; }
	}
}
