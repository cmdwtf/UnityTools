using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class LayerExtensions
	{
		// via http://answers.unity.com/answers/1712186/view.html
		public static int ToLayerIndex(this LayerMask mask)
			=> (int) Mathf.Log(mask.value, 2);
		
		public static void SetLayerRecursively(this GameObject obj, int newLayer)
		{
			obj.layer = newLayer;
       
			foreach (Transform child in obj.transform)
			{
				child.gameObject.SetLayerRecursively(newLayer);
			}
		}
	}
}
