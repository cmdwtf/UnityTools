using System;
using System.Collections;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor.RipoffParticleUI
{
	public class Header : PropertyElement
	{
		public bool ShowMenuButton { get; set; }

		public event Action<Rect> MenuButtonClicked;

		public virtual bool ShouldDrawDirectProperty { get; set; } = false;

		/// <inheritdoc />
		public Header(string propertyName, GUIContent content = null)
			: base(propertyName, content)
		{
		}

		/// <inheritdoc />
		protected override void OnDraw(Context context)
		{
			bool isRepaintEvent = Event.current.type == EventType.Repaint;

			GUIContent plusText = new("+");

			// When displaying prefab overrides, the whole UI is disabled, but we still need to be able to expand the modules to see the settings - this doesn't modify the asset
			bool wasEnabled = GUI.enabled;
			GUI.enabled = true;

			//Rect headerRect = EditorGUILayout.GetControlRect(hasLabel: false, UI.HeaderHeightPx);
			Rect headerRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, Style.HeaderHeightPx);
			headerRect = EditorGUI.IndentedRect(headerRect);

			GUI.Label(headerRect, GUIContent.none, context.Styles.moduleBgStyle);

			Rect iconRect = new(headerRect.x + Style.HeaderInteractPaddingPx, headerRect.y + Style.HeaderInteractPaddingHalfPx, Style.HeaderIconSizePx, Style.HeaderIconSizePx);

			if (isRepaintEvent)
			{
				bool iconRendered = false;

				if (Content.image != null)
				{
					GUI.DrawTexture(iconRect, Content.image, ScaleMode.StretchToFill, true);
					iconRendered = true;
				}

				if (!iconRendered)
				{
					GUI.Label(iconRect, GUIContent.none, context.Styles.moduleBgStyle);
				}

				//toggleState = EditorGUI.DropdownButton(iconRect, GUIContent.none, FocusType.Passive, GUIStyle.none);
			}

			// Button logic for plus/minus (see below for UI)
			Rect plusRect = new(headerRect.x + headerRect.width - Style.HeaderPlusWidthPaddingPx, headerRect.y + headerRect.height - Style.HeaderPlusWidthPaddingPx, Style.HeaderPlusWidthPaddingPx, Style.HeaderPlusWidthPaddingPx);
			Rect plusRectInteract = new(plusRect.x - Style.HeaderInteractPaddingPx, plusRect.y - Style.HeaderInteractPaddingPx, plusRect.width + Style.HeaderInteractPaddingPx, plusRect.height + Style.HeaderInteractPaddingPx);

			if (ShowMenuButton)
			{
				if (EditorGUI.DropdownButton(plusRectInteract, plusText, FocusType.Passive, GUIStyle.none))
				{
					MenuButtonClicked?.Invoke(plusRectInteract);
				}
			}

			GUIStyle headerLabelStyle = new(context.Styles.headerStyle);

			headerLabelStyle.alignment = TextAnchor.LowerLeft;

			IsExpanded = GUI.Toggle(headerRect, IsExpanded, new GUIContent(Content.text), headerLabelStyle);

			// Render plus/minus on top
			if (isRepaintEvent && ShowMenuButton)
			{
				GUI.Label(plusRect, GUIContent.none, context.Styles.plus);
			}

			GUILayout.Space(Style.HeaderVerticalPaddingPx); // dist to next module

			// var supportsCullingText = new GUIContent("", ParticleSystemStyles.Get().warningIcon, m_SupportsCullingTextLabel);
			//GUI.Label(infoRect, supportsCullingText);

			GUI.enabled = wasEnabled;
		}

		#region Overrides of PropertyElement

		/// <inheritdoc />
		public override void OnDrawProperties(Context context)
		{
			if (ShouldDrawDirectProperty &&
				AssociatedProperty != null)
			{
				EditorGUILayout.PropertyField(AssociatedProperty, Content, false);
			}

			base.OnDrawProperties(context);
		}

		#endregion
	}
}
