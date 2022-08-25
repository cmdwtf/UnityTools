using System.Diagnostics;

using UnityEngine;

using UnityEditor;

namespace cmdwtf.UnityTools.Editor
{
	/// <summary>
	/// A teeny editor to add buttons to test the camera projection blender.
	/// </summary>
	[CustomEditor(typeof(CameraProjectionBlender))]
	public class CameraProjectionBlenderEditor : CustomEditorBase
	{
		#region Overrides of Editor

		/// <inheritdoc />
		public override void OnInspectorGUI()
		{
			if (Application.isPlaying)
			{
				PlayingGUI();
			}

			base.OnInspectorGUI();
		}

		#endregion

		private void PlayingGUI()
		{
			var cpb = target as CameraProjectionBlender;

			if (cpb == null)
			{
				return;
			}

			GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

			if (Debugger.IsAttached)
			{
				Color oldColor = GUI.color;

				GUILayout.BeginHorizontal();

				GUI.color = Color.yellow;

				if (GUILayout.Button("Force Perspective"))
				{
					cpb.ForcePerspective();
				}

				if (GUILayout.Button("Force 'Almost Orthographic'"))
				{
					cpb.ForceAlmostOrthographic();
				}

				if (GUILayout.Button("Force Orthographic"))
				{
					cpb.ForceOrthographic();
				}

				GUILayout.EndHorizontal();

				GUI.color = oldColor;

				GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
			}

			GUILayout.BeginHorizontal();

			GUI.enabled = !cpb.FinishedPerspective;

			if (GUILayout.Button("Blend to Perspective"))
			{
				cpb.Perspective();
			}

			GUI.enabled = !cpb.FinishedOrthographic;

			if (GUILayout.Button("Blend to Orthographic"))
			{
				cpb.Orthographic();
			}

			GUI.enabled = true;

			GUILayout.EndHorizontal();

			GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
		}
	}
}
