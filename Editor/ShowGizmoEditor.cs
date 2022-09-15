using cmdwtf.UnityTools.Gizmos;

using UnityEditor;

namespace cmdwtf.UnityTools.Editor
{
	[CustomEditor(typeof(Gizmos.ShowIcon))]
	public class ShowGizmoEditor : CustomEditorBase
	{
		protected override bool ShouldHideOpenButton() => false;
	}
}
