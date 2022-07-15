using System;
using System.Linq;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	[CustomEditor(typeof(Transform))]
	[CanEditMultipleObjects]
	public class DrivenTransformEditor : CustomEditorBase
	{
		private const string PositionPropertyName = "m_LocalPosition";
		private const string PositionPropertyDisplayName = "Position";
		private const string RotationPropertyName = "m_LocalEulerAnglesHint";
		private const string RotationPropertyDisplayName = "Rotation";
		private const string ScalePropertyName = "m_LocalScale";
		private const string ScalePropertyDisplayName = "Scale";

		private Type _fallbackEditorType;
		private UnityEditor.Editor _fallbackEditor;

		private GUIContent _warningIcon;

		private void OnEnable()
		{
			_warningIcon = EditorGUIUtility.IconContent("console.warnicon.sml");
			CreateFallbackEditor();
		}

		private void OnDisable()
		{
			_warningIcon = null;
			DestroyFallback();
		}

		// thanks, https://forum.unity.com/threads/extending-instead-of-replacing-built-in-inspectors.407612/
		private void CreateFallbackEditor()
		{
			// clean up any existing one we may have.
			DestroyFallback();

			_fallbackEditorType ??= Type.GetType("UnityEditor.TransformInspector, UnityEditor");

			if (_fallbackEditorType != null)
			{
				_fallbackEditor = CreateEditor(targets, _fallbackEditorType);
			}
		}

		private void DestroyFallback()
		{
			if (_fallbackEditor == null)
			{
				return;
			}

			MethodInfo disableMethod = _fallbackEditorType.GetMethod(nameof(OnDisable), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			if (disableMethod != null)
			{
				disableMethod.Invoke(_fallbackEditor,null);
			}

			DestroyImmediate(_fallbackEditor);
			_fallbackEditor = null;
		}

		/// <inheritdoc />
		public override void OnInspectorGUI()
		{
			if (_fallbackEditor == null)
			{
				EditorGUILayout.HelpBox("Couldn't create fallback editor, doing basic inspector GUI.",
					MessageType.Error);
				base.OnInspectorGUI();
			}

			var transformDrivers = targets.Select(t => (t as Transform)?.GetComponent<ITransformDriver>())
				.Where(d => d != null)
				.ToList();

			// no transform drivers, let the base inspector handle it.
			if (transformDrivers.Count == 0)
			{
				_fallbackEditor.OnInspectorGUI();
				return;
			}

			// we have at least one transform driver,
			// disallow multi-editing with a message if we are.

			if (targets.Length > 1)
			{
				EditorGUILayout.HelpBox($"Editing multiple objects where one or more have {nameof(ITransformDriver)} components is not supported.", MessageType.None);
				return;
			}

			ITransformDriver driver = transformDrivers.First();

			bool willDrivePosition = driver.IsDrivingPosition;
			bool willDriveRotation = driver.IsDrivingRotation;
			bool willDriveScale = driver.IsDrivingScale;

			// we *can* drive components, but we aren't,
			// so we can just use the fallback editor.
			if (!driver.IsDrivingAnyTransformComponent)
			{
				_fallbackEditor.OnInspectorGUI();
				return;
			}

			bool playing = EditorApplication.isPlaying;
			string niceDriverName = ObjectNames.NicifyVariableName(driver.GetType().Name);

			EditorGUILayout.HelpBox(
				$"Some values {(playing ? "are" : "will be")} driven by {niceDriverName}",
				MessageType.None);

			EditorGUI.BeginChangeCheck();

			GUI.enabled = !willDrivePosition || !playing;
			DoProperty(PositionPropertyName, PositionPropertyDisplayName, willDrivePosition);

			GUI.enabled = !willDriveRotation || !playing;
			DoProperty(RotationPropertyName, RotationPropertyDisplayName, willDriveRotation);

			GUI.enabled = !willDriveScale || !playing;
			DoProperty(ScalePropertyName, ScalePropertyDisplayName, willDriveScale);

			GUI.enabled = true;

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		private void DoProperty(string targetProperty, string propertyDisplayName, bool isControlled, bool includeChildren = true)
		{
			if (serializedObject.FindProperty(targetProperty) is { } property)
			{
				GUIContent guiContent = new()
				{
					text = propertyDisplayName ?? property.displayName ?? property.name,
					tooltip = isControlled ? $"{propertyDisplayName} is being controlled by a component." : property.tooltip ,
					image = isControlled ? _warningIcon.image : null,
				};

				EditorGUILayout.PropertyField(property, guiContent, includeChildren);
			}
			else
			{
				EditorGUILayout.HelpBox($"Couldn't find property: {targetProperty}", MessageType.Warning);
			}
		}
	}
}
