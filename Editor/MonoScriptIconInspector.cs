using System;
using System.Reflection;

using UnityEditor;

using UnityEngine;

using Object = UnityEngine.Object;

namespace cmdwtf.UnityTools.Editor
{
	[CustomEditor(typeof(MonoScript))]
	[CanEditMultipleObjects]
	public class MonoScriptIconInspector : UnityEditor.Editor
	{
		private UnityEditor.Editor _fallbackEditor;
		private Type _fallbackEditorType;
		private SerializedProperty _iconProperty;

		private void OnEnable()
		{
			Assembly asm = typeof(UnityEditor.Editor).Assembly;

			_fallbackEditorType = asm.GetType($"{nameof(UnityEditor)}.MonoScriptInspector") ??
									   asm.GetType($"{nameof(UnityEditor)}.GenericInspector");


			if (_fallbackEditorType != null)
			{
				_fallbackEditor = CreateEditor(targets, _fallbackEditorType);
			}

			if (_fallbackEditor == null || _fallbackEditorType == null)
			{
				Debug.LogWarning("Failed to create fallback editor, couldn't find expected types.");
			}

			_iconProperty = serializedObject.FindProperty("m_Icon");
		}

		private void OnDisable()
		{
			if (_fallbackEditor != null)
			{
				DestroyImmediate(_fallbackEditor);
			}

			_fallbackEditor = null;
		}

		public override void OnInspectorGUI()
        {
			EditorGUILayout.BeginHorizontal();

			serializedObject.UpdateIfRequiredOrScript();

			GUIContent iconGuiContent = new("Script Icon", "The icon to set for the script.");
			GUIContent clearIconContent = new("Clear Icon", "Remove the assigned icon from the script.");

            EditorGUILayout.PropertyField(_iconProperty, iconGuiContent, GUILayout.ExpandWidth(true));

			if (GUILayout.Button("Apply", GUILayout.ExpandWidth(false)))
			{
				SetIconOnTargets(_iconProperty.objectReferenceValue as Texture2D);
			}

			if (EditorGUILayoutEx.InlineHamburgerMenuButton())
			{
				GenericMenu menu = new();

				if (_iconProperty.objectReferenceValue != null)
				{
					menu.AddItem(clearIconContent, false, () => SetIconOnTargets(null));
				}
				else
				{
					menu.AddDisabledItem(clearIconContent, false);
				}

				menu.ShowAsContext();
			}

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

			if (_fallbackEditor != null)
			{
				_fallbackEditor.OnInspectorGUI();
			}
        }

		private void SetIconOnTargets(Texture2D iconTexture)
		{
			foreach (Object t in targets)
			{
				if (t is not MonoScript ms)
				{
					Debug.LogWarning($"Target object is {t.GetType().FullName}, not {nameof(MonoScript)}, can't assign icon.");
					continue;
				}

				if (AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(ms)) is not MonoImporter importer)
				{
					Debug.LogWarning($"Failed to get importer for {ms.name}.");
					continue;
				}

				importer.SetIcon(iconTexture);
				importer.SaveAndReimport();
			}
		}
	}
}
