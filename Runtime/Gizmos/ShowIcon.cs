using UnityEngine;

namespace cmdwtf.UnityTools.Gizmos
{
	/// <summary>
	/// A small component that causes an gizmo icon to draw.
	/// </summary>
	public class ShowIcon : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField]
		private string gizmoName = "d_Favorite@2x";

		private void OnDrawGizmos()
		{
			Color originalColor = UnityEngine.Gizmos.color;
			var transparent = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
			UnityEngine.Gizmos.color = transparent;
			OnDrawGizmosSelected();
			UnityEngine.Gizmos.color = originalColor;
		}

		private void OnDrawGizmosSelected()
			=> UnityEngine.Gizmos.DrawIcon(transform.position, gizmoName);

#endif // UNITY_EDITOR
	}
}
