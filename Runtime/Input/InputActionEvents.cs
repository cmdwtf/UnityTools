using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace cmdwtf.UnityTools.Input
{
	public class InputActionEvents : MonoBehaviour
	{
		[SerializeField]
		internal InputActionMap actions;

		[SerializeField]
		internal List<InputActionEvent> callbacks;

		private Dictionary<Guid, InputActionEvent> _callbacksLookup;

		private void Awake()
		{
			_callbacksLookup = new Dictionary<Guid, InputActionEvent>();
			foreach (InputActionEvent e in callbacks)
			{
				_callbacksLookup.Add(e.inputActionGuid, e);
			}

			actions.actionTriggered += HandleActionTriggered;
		}

		private void OnEnable()
		{
			foreach (InputAction a in actions)
			{
				a.Enable();
			}
		}

		private void OnDisable()
		{
			foreach (InputAction a in actions)
			{
				a.Disable();
			}
		}

		private void HandleActionTriggered(InputAction.CallbackContext obj)
		{
			if (!_callbacksLookup.TryGetValue(obj.action.id, out InputActionEvent e))
			{
				return;
			}

			switch (obj.phase)
			{
				case InputActionPhase.Started:
					e.started?.Invoke();
					break;
				case InputActionPhase.Performed:
					e.performed?.Invoke();
					break;
				case InputActionPhase.Canceled:
					e.cancelled?.Invoke();
					break;
				case InputActionPhase.Disabled:
				case InputActionPhase.Waiting:
				default:
					// nothing to do
					break;
			}
		}
	}
}
