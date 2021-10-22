using UnityEditor;

namespace cmdwtf.UnityTools.Editor
{
	[CustomEditor(typeof(ShowGizmo))]
	public class ShowGizmoEditor : CustomEditorBase
	{
		protected override bool ShouldHideOpenButton() => false;
	}
}