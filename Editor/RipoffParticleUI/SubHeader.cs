using System;

using UnityEditor;

using UnityEngine;

using Random = System.Random;

namespace cmdwtf.UnityTools.Editor.RipoffParticleUI
{
	/// <summary>
	/// A smaller header with an optional checkbox.
	/// </summary>
	public class SubHeader : Header
	{
		/// <summary>
		/// True to show the checkbox on the subheader.
		/// </summary>
		public bool ShowCheckbox { get; set; } = true;

		/// <summary>
		/// Raised when the checkbox is clicked.
		/// </summary>
		public event Action<SubHeader> CheckboxClicked;

		/// <summary>
		/// If true and the <see cref="CheckboxSubPropertyName"/> doesn't refer to a
		/// valid property, will show the checkbox in the checked state.
		/// </summary>
		public bool FallbackChecked { get; set; } = false;

		/// <summary>
		/// The serialized property name of a boolean property that is associated with the checkbox on the header.
		/// </summary>
		public string CheckboxSubPropertyName { get; set; }

		private SerializedProperty _checkboxProperty;

		/// <inheritdoc />
		public override bool ShouldDrawDirectProperty { get; set; } = true;

		/// <inheritdoc />
		public SubHeader(string propertyName, GUIContent content = null)
			: base(propertyName, content)
		{
		}

		#region Overrides of Element

		/// <inheritdoc />
		protected override void OnBeforeDraw(Context context)
		{
			base.OnBeforeDraw(context);

			if (!string.IsNullOrWhiteSpace(CheckboxSubPropertyName))
			{
				_checkboxProperty = AssociatedProperty.FindPropertyRelative(CheckboxSubPropertyName);
			}
		}

		/// <inheritdoc />
		protected override void OnDraw(Context context)
		{
			bool wasEnabled = GUI.enabled;
			GUI.enabled = true;

			Rect headerRect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, Style.SubHeaderHeightPx);
			headerRect = EditorGUI.IndentedRect(headerRect);

			// Button logic for enabledness (see below for Style)
			Rect checkMarkRect = new(headerRect.x + Style.SubHeaderInteractPaddingPx,
				headerRect.y + Style.SubHeaderInteractPaddingHalfPx, Style.CheckmarkSquarePx, Style.CheckmarkSquarePx);

			if (ShowCheckbox)
			{
				if (GUI.Button(checkMarkRect, GUIContent.none, GUIStyle.none))
				{
					if (_checkboxProperty != null)
					{
						_checkboxProperty.boolValue = !_checkboxProperty.boolValue;
					}

					CheckboxClicked?.Invoke(this);
				}
			}

			GUIContent label = Content;

			if (_checkboxProperty != null)
			{
				label = EditorGUI.BeginProperty(headerRect, Content, _checkboxProperty);
			}

			IsExpanded = GUI.Toggle(headerRect, IsExpanded, label, context.Styles.subHeaderStyle);

			if (_checkboxProperty != null)
			{
				EditorGUI.EndProperty();
			}

			// Render checkmark on top (logic: see above)
			if (ShowCheckbox)
			{
				GUIStyle style = EditorGUI.showMixedValue ? context.Styles.toggleMixed : context.Styles.toggle;
				GUI.Toggle(checkMarkRect, _checkboxProperty?.boolValue ?? FallbackChecked, GUIContent.none, style);
			}

			// var supportsCullingText = new GUIContent("", ParticleSystemStyles.Get().warningIcon, m_SupportsCullingTextLabel);
			//GUI.Label(infoRect, supportsCullingText);

			GUILayout.Space(Style.HeaderVerticalPaddingPx); // dist to next module

			GUI.enabled = wasEnabled;
		}

		#region Overrides of PropertyElement

		/// <inheritdoc />
		protected override void OnAfterDraw(Context context)
			=> _checkboxProperty = null;

		#endregion

		#endregion
	}
}
