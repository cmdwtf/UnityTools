using System;
using System.Collections.Generic;
using System.Linq;

using cmdwtf.UnityTools.Dynamics;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor.Dynamics
{
	internal class DynamicsTransformEditorDropdown : AdvancedDropdown
	{
		private const string PositionProperty = nameof(DynamicsTransform.position);
		private const string RotationProperty = nameof(DynamicsTransform.rotation);
		private const string ScaleProperty = nameof(DynamicsTransform.scale);
		private const string EnabledProperty = nameof(DynamicsTransformMutator.enabled);
		private const string ModeProperty = nameof(DynamicsTransformMutator.mode);

		private readonly SerializedObject _target;

		private readonly List<Action<string>> _actions = new();
		private readonly List<string> _actionData = new();

		private DynamicsTransformEditorDropdown(SerializedObject target, AdvancedDropdownState s) : base(s)
		{
			_target = target;
		}

		public static void Show(Rect where, SerializedObject target)
		{
			DynamicsTransformEditorDropdown dropdown = new(target, new AdvancedDropdownState());
			dropdown.Show(where);
		}

		private int AddAction(Action<string> a, string data)
		{
			_actions.Add(a);
			_actionData.Add(data);
			return _actions.Count - 1;
		}

		#region Overrides of AdvancedDropdown

		// well this is a disaster to read. should be cleaned up at some point.
		/// <inheritdoc />
		protected override AdvancedDropdownItem BuildRoot()
		{
			_actions.Clear();
			AdvancedDropdownItem root = new("Dynamics System");
			AdvancedDropdownItem removeSubmenu = new("Remove Existing");

			void CreateAddOrRemoveItem(bool canDrive, string item, Action<string> add, Action<string> remove)
			{
				string niceName = ObjectNames.NicifyVariableName(item);
				if (canDrive)
				{
					AdvancedDropdownItem c = new($"Remove {niceName}")
					{
						id = AddAction(remove, item),
					};

					removeSubmenu.AddChild(c);
				}
				else
				{
					AdvancedDropdownItem c = new($"Add {niceName} Dynamics")
					{
						id = AddAction(add, item),
					};

					root.AddChild(c);
				}
			}

			void EnableAction(string propertyName)
			{
				_target.TrySetProperty($"{propertyName}.{EnabledProperty}", true);
				_target.TrySetProperty($"{propertyName}.{ModeProperty}", DynamicsTransform.DefaultMode);
			}

			void DisableAction(string propertyName)
			{
				_target.TrySetProperty($"{propertyName}.{EnabledProperty}", false);
				_target.TrySetProperty($"{propertyName}.{ModeProperty}", DynamicsUpdateMode.Ignored);
			}

			bool ShouldPropertyBeRemovable(string propertyName)
			{
				if (_target.TryGetProperty($"{propertyName}.{EnabledProperty}", out bool enabled) && enabled)
				{
					return true;
				}

				if (_target.TryGetProperty($"{propertyName}.{ModeProperty}", out DynamicsUpdateMode mode))
				{
					return mode != DynamicsUpdateMode.Ignored;
				}

				return false;
			}

			void CreateItemForProperty(string propertyName)
			{
				bool canDrive = ShouldPropertyBeRemovable(propertyName);
				CreateAddOrRemoveItem(canDrive, propertyName, EnableAction, DisableAction);
			}

			CreateItemForProperty(PositionProperty);
			CreateItemForProperty(RotationProperty);
			CreateItemForProperty(ScaleProperty);

			// if we don't have any to remove, just return the root menu.
			if (!removeSubmenu.children.Any())
			{
				return root;
			}

			// if we don't have any to add, change the text
			// on the remove, and return it as root.
			if (!root.children.Any())
			{
				removeSubmenu.name = "Remove Dynamics";
				return removeSubmenu;
			}

			// otherwise add a separator and the remove submenu as a child

			root.AddSeparator();
			root.AddChild(removeSubmenu);

			return root;
		}

		/// <inheritdoc />
		protected override void ItemSelected(AdvancedDropdownItem item)
			=> _actions[item.id]?.Invoke(_actionData[item.id]);

		#endregion
	}
}
