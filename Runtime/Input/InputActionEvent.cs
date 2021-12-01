using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace cmdwtf.UnityTools.Input
{
	[Serializable]
	internal class InputActionEvent
	{
		[HideInInspector]
		public string name;

		[HideInInspector]
		public SerializableGuid inputActionGuid;

		public UnityEvent started;
		public UnityEvent performed;
		public UnityEvent cancelled;

		internal InputActionEvent(InputAction baseAction)
		{
			name = baseAction.name.SpaceCamelCase();
			inputActionGuid = baseAction.id;
		}
	}
}
