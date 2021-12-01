using UnityEngine;

namespace cmdwtf.UnityTools.UI
{
	public interface IPageDescription
	{
		string Name { get; set; }
		Page Page { get; set; }
		GameObject Button { get; set; }
	}
}
