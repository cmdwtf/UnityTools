using System;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	public abstract class CustomPopupWindowContent<TDerived>
		: PopupWindowContent where TDerived : CustomPopupWindowContent<TDerived>
	{
		public event Action<TDerived> Opened;
		public event Action<TDerived> Closed;
		public event Action<TDerived> EditorChanged;
		public event Action<TDerived> CancelClicked;
		public event Action<TDerived> AcceptClicked;

		public GUIContent Title { get; set; } = GUIContent.none;
		public GUIStyle TitleStyle { get; set; } = default;

		public string CancelButtonText { get; set; }= "Cancel";
		public string AcceptButtonText { get; set; } = "Accept";

		public bool ShowCancelButton { get; set; } = true;
		public bool ShowAcceptButton { get; set; } = true;

		public float Width
		{
			get => _size.x;
			set => _size.x = value;
		}

		private TDerived Derived => this as TDerived;

		private Vector2 _size;

		public CustomPopupWindowContent(string title = null)
		{
			_size = base.GetWindowSize();

			if (!string.IsNullOrWhiteSpace(title))
			{
				Title = new GUIContent(title);
			}
		}

		public override void OnGUI(Rect rect)
		{
			EditorGUI.BeginChangeCheck();

			if (Title != GUIContent.none)
			{
				EditorGUILayoutEx.DropShadowLabel(Title, TitleStyle);
			}

			ShouldDrawGUI(rect);

			DrawDefaultGUI(rect);

			if (EditorGUI.EndChangeCheck())
			{
				EditorChanged?.Invoke(Derived);
			}

			if (Event.current.type == EventType.Repaint)
			{
				Vector2 lastControlMax = GUILayoutUtility.GetLastRect().max;
				_size.y = lastControlMax.y + EditorGUIUtility.standardVerticalSpacing;
			}
		}

		private void DrawDefaultGUI(Rect rect)
		{
			if (ShowCancelButton || ShowAcceptButton)
			{
				EditorGUILayout.BeginHorizontal();

				if (ShowCancelButton && GUILayout.Button(CancelButtonText))
				{
					OnCancelClicked();
				}

				if (ShowAcceptButton && GUILayout.Button(AcceptButtonText))
				{
					OnAcceptClicked();
				}

				EditorGUILayout.EndHorizontal();
			}
		}

		protected abstract void ShouldDrawGUI(Rect rect);

		public override void OnOpen() => Opened?.Invoke(Derived);

		public override void OnClose() => Closed?.Invoke(Derived);

		protected virtual void OnAcceptClicked()
		{
			AcceptClicked?.Invoke(Derived);
			editorWindow.Close();
		}

		protected virtual void OnCancelClicked()
		{
			CancelClicked?.Invoke(Derived);
			editorWindow.Close();
		}

		public override Vector2 GetWindowSize() => _size;
	}
}
