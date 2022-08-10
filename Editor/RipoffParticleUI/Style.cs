using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor.RipoffParticleUI
{
	/// <summary>
	/// Styles used by the <see cref="RipoffParticleUI"/> classes.
	/// Many values and names borrowed from Unity's "Shuriken" UI bits.
	/// </summary>
	public class Style
	{
		public const float HeaderHeightPx = 25f;
		public const float SubHeaderHeightPx = 15f;
		public const float CheckmarkSquarePx = 13f;
		public const float HeaderPlusWidthPaddingPx = 10;
		public const float HeaderInteractPaddingPx = 4;
		public const float HeaderInteractPaddingHalfPx = HeaderInteractPaddingPx / 2f;
		public const float SubHeaderInteractPaddingPx = 2;
		public const float SubHeaderInteractPaddingHalfPx = SubHeaderInteractPaddingPx / 2f;
		public const float HeaderIconSizePx = 21f;
		public const float HeaderVerticalPaddingPx = 1f;
		public const float ToggleSquarePx = 12f;
		public const float TogglePaddingPx = 1f;
		public const float DropdownToggleSquarePx = 13f;
		public const float DropdownTogglePaddingPx = 1f;

		public GUIStyle label;
		public GUIStyle editableLabel;
		public GUIStyle objectField;
		public GUIStyle numberField;
		public GUIStyle subHeaderStyle;
		public GUIStyle popup;
		public GUIStyle headerStyle;
		public GUIStyle effectBgStyle;
		public GUIStyle moduleBgStyle;
		public GUIStyle plus;
		public GUIStyle minus;
		public GUIStyle checkmark;
		public GUIStyle checkmarkMixed;
		public GUIStyle minMaxCurveStateDropDown;
		public GUIStyle toggle;
		public GUIStyle toggleMixed;
		public GUIStyle selectionMarker;
		public GUIStyle toolbarButtonLeftAlignText;
		public GUIStyle modulePadding;
		public GUIStyle customDataWindow;

		public GUIStyle presetButton;
		public GUIContent presetIcon;

		public Style()
		{
			label = "ShurikenLabel";
			editableLabel = "ShurikenEditableLabel";
			objectField = "ShurikenObjectField";
			numberField = "ShurikenValue";
			subHeaderStyle = "ShurikenModuleTitle";
			popup = "ShurikenPopUp";
			headerStyle = "ShurikenEmitterTitle";
			headerStyle.fontStyle = FontStyle.Bold;
			effectBgStyle = "ShurikenEffectBg";
			moduleBgStyle = "ShurikenModuleBg";
			plus = "ShurikenPlus";
			minus = "ShurikenMinus";
			checkmark = "ShurikenCheckMark";
			checkmarkMixed = "ShurikenCheckMarkMixed";
			minMaxCurveStateDropDown = "ShurikenDropdown";
			toggle = "ShurikenToggle";
			toggleMixed = "ShurikenToggleMixed";
			selectionMarker = "IN ThumbnailShadow";
			toolbarButtonLeftAlignText = "ToolbarButton";
			modulePadding = new() {padding = new RectOffset(3, 3, 4, 2),};
			customDataWindow = new(GUI.skin.window) {font = EditorStyles.miniFont};

			presetButton = new(EditorStyles.iconButton)
			{
				fixedHeight = minMaxCurveStateDropDown.fixedHeight,
				fixedWidth = minMaxCurveStateDropDown.fixedWidth,
				//padding = minMaxCurveStateDropDown.padding,
				border = minMaxCurveStateDropDown.border,
			};
			presetIcon = EditorGUIUtility.IconContent("Preset.Context");
			presetIcon.tooltip = "Presets";
		}
	}
}
