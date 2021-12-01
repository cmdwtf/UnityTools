using System;
using System.Collections.Generic;
using System.Linq;using cmdwtf.UnityTools.Input;

using UnityEditor;

using UnityEngine;
using UnityEngine.InputSystem;

using InputActionEvents = cmdwtf.UnityTools.Input.InputActionEvents;

namespace cmdwtf.UnityTools.Editor
{
	[CustomEditor(typeof(InputActionEvents))]
	public class InputActionEventsEditor  : CustomEditorBase
	{
		public static bool ShowDebug { get; set; }

		private int _focusedTab;

		public override void OnInspectorGUI()
		{
			var root = target as InputActionEvents;

			if (root == null)
			{
				throw new ArgumentNullException(nameof(root));
			}

			serializedObject.UpdateIfRequiredOrScript();

			_focusedTab = GUILayout.SelectionGrid(_focusedTab, new string[] { "Actions", "Callbacks" }, 2);

			switch (_focusedTab)
			{
				case 0:
					GUIActions(root);
					break;
				case 1:
					GUICallbacks(root);
					break;
			}

			if (ShowDebug)
			{
				DebugShowOrphans(root);
			}
		}

		private void GUIActions(InputActionEvents root)
		{
			Rect r = EditorGUILayout.GetControlRect();
			EditorGUI.DropShadowLabel(r, "Actions");

			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(root.actions)));
			serializedObject.ApplyModifiedProperties();
		}

		private void GUICallbacks(InputActionEvents root)
		{
			Rect r = EditorGUILayout.GetControlRect();
			EditorGUI.DropShadowLabel(r, "Callbacks");

			SerializedProperty callbacks = serializedObject.FindProperty(nameof(root.callbacks));

			var toRemove = new List<InputActionEvent>();

			foreach (InputAction action in root.actions.actions)
			{
				string display = action.name.SpaceCamelCase();
				int index = root.callbacks.FindIndex(iae => iae.inputActionGuid == action.id);

				if (index < 0)
				{
					// doesn't exist, prompt user to add
					GUITools.PushColor(Colors.info);
					if (GUILayout.Button($"Add Events for {display}"))
					{
						var toAdd = new InputActionEvent(action);
						root.callbacks.Add(toAdd);
						serializedObject.Update();

						SerializedProperty callbackProperty = callbacks.GetArrayElementAtIndex(callbacks.arraySize - 1);
						callbackProperty.isExpanded = true;

						break;
					}

					GUITools.PopColor();
				}
				else
				{
					SerializedProperty callbackProperty = callbacks.GetArrayElementAtIndex(index);

					if (callbackProperty.isExpanded)
					{
						EditorGUILayout.PropertyField(callbackProperty);

						GUITools.PushColor(Colors.danger);

						// allow the deletion of this item too.
						if (GUILayout.Button($"Delete {display} Events"))
						{
							toRemove.Add(root.callbacks[index]);
							break;
						}

						GUITools.PopColor();
					}
					else
					{
						callbackProperty.isExpanded = EditorGUILayout.Foldout(callbackProperty.isExpanded, display, toggleOnLabelClick: true);
					}
				}
			}

			serializedObject.ApplyModifiedProperties();

			foreach (InputActionEvent iae in toRemove)
			{
				root.callbacks.Remove(iae);
			}
		}

		private void DebugShowOrphans(InputActionEvents root)
		{
			List<InputActionEvent> orphans = new List<InputActionEvent>();

			foreach (InputActionEvent iae in root.callbacks)
			{
				bool found = root.actions.FindAction(iae.inputActionGuid) != null;

				if (!found)
				{
					EditorGUILayout.LabelField($"Orphaned: {iae.name} - {iae.inputActionGuid}");
					orphans.Add(iae);
				}
			}

			if (!orphans.Any())
			{
				return;
			}

			if (GUILayout.Button("Remove Orphans"))
			{
				foreach (InputActionEvent iae in orphans)
				{
					root.callbacks.Remove(iae);
				}
			}
		}
	}
}
