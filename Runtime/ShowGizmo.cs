using UnityEngine;

namespace cmdwtf.UnityTools
{
	public class ShowGizmo : MonoBehaviour
	{
		[SerializeField]
		private string gizmoName = "d_Favorite@2x";
	
		private void OnDrawGizmos()
		{
			Color originalColor = Gizmos.color;
			var transparent = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
			Gizmos.color = transparent;
			OnDrawGizmosSelected();
			Gizmos.color = originalColor;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawIcon(transform.position, gizmoName);
		}
	}
}
