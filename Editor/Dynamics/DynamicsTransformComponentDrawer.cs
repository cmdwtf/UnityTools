using System;

using cmdwtf.UnityTools.Dynamics;
using cmdwtf.UnityTools.Editor.RipoffParticleUI;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor.Dynamics
{
	public abstract class DynamicsTransformComponentDrawer : CustomPropertyDrawer
	{
		private static readonly Color[] LineColors = new[]
		{
			Color.red, Color.green, Color.blue, Color.cyan, Color.yellow, Color.magenta, Color.white
		};

		private static readonly Color DisabledLineColor = Color.gray;

		private protected readonly DynamicsGraphRenderer _renderer;
		private protected SerializedProperty _currentProperty;
		private protected IMultidimensionalDynamicsProvider _currentDynamics;

		protected float GraphHeightMultiplier { get; set; } = 1f;
		internal int LineColorIndexOffset { get; set; } = 0;

		protected const int DisplayHeightGraphLines = 4;
		protected static readonly float DisplayHeightGraphPx = (DisplayHeightGraphLines * EditorGUIUtility.singleLineHeight)
															 ;//+ (DisplayHeightGraphLines * EditorGUIUtility.standardVerticalSpacing);

		protected static readonly float DisplayHeightGraphPaddingPx = EditorGUIUtility.singleLineHeight;

		private string _rootPropertyName;

		private protected Style _styles;

		public DynamicsTransformComponentDrawer()
		{
			_renderer = new DynamicsGraphRenderer();
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			_styles ??= new Style();

			string[] propSplits = property.propertyPath.Split('.', 2);

			if (propSplits.Length > 2)
			{
				throw new ArgumentException("Unable to split property path.", nameof(property));
			}

			_rootPropertyName = ObjectNames.NicifyVariableName(propSplits[0]);

			property.serializedObject.Update();

			_currentProperty = property;
			_currentDynamics = property.GetValue<IMultidimensionalDynamicsProvider>();

			if (_currentDynamics != null)
			{
				OnUpdateLines(_currentDynamics);
			}
			else
			{
				EditorGUI.HelpBox(position.EditorGUILineHeight(), $"Unhandled IDynamicsTransform type: {_currentDynamics?.GetType().FullName}", MessageType.Warning);
			}

			Rect propertyFoldoutRect = position.CollapseToRight(Style.DropdownToggleSquarePx).EditorGUILineHeight();

			if (EditorGUI.DropdownButton(propertyFoldoutRect, GUIContent.none,
										 FocusType.Keyboard, _styles.minMaxCurveStateDropDown))
			{
				property.isExpanded = !property.isExpanded;
			}

			int graphLines = property.isExpanded
								 ? DisplayHeightGraphLines
								 : 1;

			position = position.EditorGUILineHeight(graphLines);
			position.width -= (Style.DropdownToggleSquarePx + Style.DropdownTogglePaddingPx);

			if (Event.current.type == EventType.Repaint)
			{
				OnRenderGraph(position, _currentDynamics, property.isExpanded);
			}

			if (!property.isExpanded)
			{
				return;
			}

			Rect presetButtonRect = propertyFoldoutRect;
			presetButtonRect.EditorGUINextLine();

			if (property.isExpanded &&
				GUI.Button(presetButtonRect.EditorGUILineHeight(), _styles.presetIcon, _styles.presetButton))
			{
				DynamicsTransformComponentPresetsPopup popup = new(_currentDynamics?.GetDynamicsSystemForDimension(0));
				popup.PresetSelected += OnApplyPreset;

				PopupWindow.Show(presetButtonRect, popup);
			}

			position.y += position.height + DisplayHeightGraphPaddingPx + EditorGUIUtility.standardVerticalSpacing;

			position.height = OnGetShownPropertyHeight(property);

			OnRenderInspector(position, property);
		}

		protected abstract void OnUpdateLines(IMultidimensionalDynamicsProvider mdDynamics);
		protected abstract void OnRenderGraph(Rect position, IMultidimensionalDynamicsProvider mdDynamics, bool isExpanded);
		protected abstract void OnRenderInspector(Rect position, SerializedProperty property);
		protected abstract float OnGetShownPropertyHeight(SerializedProperty property);
		protected abstract void OnApplyPreset(IDynamicsPreset preset);

		protected void UpdateLine(ISimulatableDynamicsSystem simmable, bool enabled, int lineIndex)
		{
			DynamicsSimulationConfig config = new(simmable);
			string lineKey = CreateLineKey(lineIndex);
			Color lineColor = enabled ? GetLineColor(lineIndex + LineColorIndexOffset) : DisabledLineColor;
			_renderer.UpdateDynamicsLine(config, lineKey, lineColor);
		}

		protected string CreateLineKey(int lineIndex) => $"{_rootPropertyName}.{lineIndex}";

		private static Color GetLineColor(int lineIndex)
		{
			if (lineIndex < 0 || lineIndex >= LineColors.Length)
			{
				Debug.LogWarning($"Line index ({lineIndex}) was out of predefined color range (0-{LineColors.Length-1})");
				return Color.white;
			}

			return LineColors[lineIndex];
		}

		#region Overrides of CustomPropertyDrawer

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!property.isExpanded)
			{
				return EditorGUIUtility.singleLineHeight;
			}

			float baseHeight = OnGetShownPropertyHeight(property) + EditorGUIUtility.standardVerticalSpacing;
			float graphHeight = DisplayHeightGraphPx + DisplayHeightGraphPaddingPx;
			return (baseHeight + (graphHeight * GraphHeightMultiplier));
		}

		protected override int GetPropertyLineCount(SerializedProperty property, GUIContent label)
		{
			float height = GetPropertyHeight(property, label);
			PropertyLineCount = (int)(height / Constants.StandardLineHeight);
			return (int)(PropertyLineCount);
		}

		#endregion
	}
}
