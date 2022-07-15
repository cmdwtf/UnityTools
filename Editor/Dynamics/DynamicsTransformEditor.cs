using System.Collections.Generic;

using cmdwtf.UnityTools.Dynamics;
using cmdwtf.UnityTools.Editor.RipoffParticleUI;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor.Dynamics
{
	[CustomEditor(typeof(DynamicsTransform))]
	public class DynamicsTransformEditor : CustomEditorBase
	{
		private UIRoot _ui;
		private DynamicsTransform _lastTarget;

		/// <inheritdoc />
		public override void OnInspectorGUI()
		{
			_lastTarget = serializedObject.targetObject as DynamicsTransform;

			_ui = BuildUI();

			EditorGUI.BeginChangeCheck();

			_ui.Draw(serializedObject);

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		private UIRoot BuildUI()
		{
			UIRoot ui = new();

			GUIContent headerContent = EditorGUIUtility.IconContent("Transform Icon");
			headerContent.text = DynamicsTransformStateToHeaderText(_lastTarget);
			Header main = new(null, headerContent)
			{
				IsExpanded = _lastTarget.IsDrivingAnyTransformComponent,
				ShowMenuButton = true,
			};

			main.MenuButtonClicked += r => DynamicsTransformEditorDropdown.Show(r, serializedObject);

			ui.Add(main);

			main.AddSimpleProperty(nameof(DynamicsTransform.targetTransform));

			SubHeader pos = new(nameof(DynamicsTransform.position));
			pos.CheckboxSubPropertyName = nameof(DynamicsPosition.enabled);
			pos.UpdateVisibility += e => _lastTarget.CanDrivePosition;

			SubHeader rot = new(nameof(DynamicsTransform.rotation));
			rot.CheckboxSubPropertyName = nameof(DynamicsRotation.enabled);
			rot.UpdateVisibility += e => _lastTarget.CanDriveRotation;

			SubHeader sca = new(nameof(DynamicsTransform.scale));
			sca.CheckboxSubPropertyName = nameof(DynamicsScale.enabled);
			sca.UpdateVisibility += e => _lastTarget.CanDriveScale;

			main.Add(pos);
			main.Add(rot);
			main.Add(sca);

			return ui;
		}

		//private static string DynamicsTransformStateToHeaderText(bool drivingPos, bool drivingRot, bool drivingSca)
		private static string DynamicsTransformStateToHeaderText(DynamicsTransform dt)
		{
			if (!dt.IsDrivingAnyTransformComponent)
			{
				return "Dynamics Transform (Inactive)";
			}

			List<string> driving = new();

			if (dt.IsDrivingPosition)
			{
				driving.Add("Position");
			}

			if (dt.IsDrivingRotation)
			{
				driving.Add("Rotation");
			}

			if (dt.IsDrivingScale)
			{
				driving.Add("Scale");
			}

			return $"Dynamics: {string.Join(", ", driving)}";
		}
	}
}
