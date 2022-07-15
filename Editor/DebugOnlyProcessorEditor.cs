using cmdwtf.UnityTools.Input;

using UnityEditor;

using UnityEngine;
using UnityEngine.InputSystem.Editor;

namespace cmdwtf.UnityTools.Editor
{
	public class DebugOnlyProcessorEditor : InputParameterEditor<DebugOnlyProcessor>
	{
		private const string Prefix = "Allow In ";
		private readonly GUIContent _inEditorContent = new GUIContent(Prefix + "Editor");
		private readonly GUIContent _inDebugBuildContent = new GUIContent(Prefix + "Debug Build");
		private readonly GUIContent _inReleaseBuildContent = new GUIContent(Prefix + "Release Build");

		public override void OnGUI()
		{
			target._allowInReleaseBuild = EditorGUILayout.Toggle(_inReleaseBuildContent, target._allowInReleaseBuild);

			GUITools.PushEnabled(!target._allowInReleaseBuild);

			target._allowInDebugBuild = EditorGUILayout.Toggle(_inDebugBuildContent, target._allowInDebugBuild || target._allowInReleaseBuild);

			GUI.enabled = !target._allowInDebugBuild;

			target._allowInEditor = EditorGUILayout.Toggle(_inEditorContent, target._allowInEditor || target._allowInDebugBuild);

			GUITools.PopEnabled();
		}
	}
}
