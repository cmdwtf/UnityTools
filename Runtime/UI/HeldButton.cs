using System;

#if USE_UNITY_UI

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace cmdwtf.UnityTools.UI
{
	/// <summary>
	/// An extension of the <see cref="UnityEngine.UI.Button">standard button</see> that
	/// also reports if it is or is not being held.
	/// </summary>
	[AddComponentMenu("UI/Held Button", 30)]
	public class HeldButton : UnityEngine.UI.Button
	{
		/// <summary>
		/// Function definition for a button press or release event.
		/// </summary>
		[Serializable]
		public class ButtonStateEvent : UnityEvent<HeldButton> {}

		// Event delegates triggered on press
		[SerializeField]
		internal ButtonStateEvent _onPress = new ButtonStateEvent();

		// Event delegates triggered on release
		[SerializeField]
		internal ButtonStateEvent _onRelease = new ButtonStateEvent();

		protected HeldButton()
		{ }

		/// <summary>
		/// Returns true if the button is in the pressed state.
		/// </summary>
		public bool isHeld => currentSelectionState == SelectionState.Pressed;

		public ButtonStateEvent onPress
		{
			get { return _onPress; }
			set { _onPress = value; }
		}

		public ButtonStateEvent onRelease
		{
			get { return _onRelease; }
			set { _onRelease = value; }
		}

		#region Overrides of Selectable

		/// <inheritdoc />
		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
			_onPress.Invoke(this);
		}

		/// <inheritdoc />
		public override void OnPointerUp(PointerEventData eventData)
		{
			base.OnPointerUp(eventData);
			_onRelease.Invoke(this);
		}

		#endregion
	}
}

#endif // USE_UNITY_UI
