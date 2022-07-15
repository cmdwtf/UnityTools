using cmdwtf.UnityTools.Dynamics;

using System.Linq;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor.Dynamics
{
	[UnityEditor.CustomPropertyDrawer(typeof(DynamicsVector3), useForChildren: true)]
	public class DynamicsVector3Drawer : DynamicsTransformComponentDrawer
	{
		private const int MaximumPropertyChoices = 3;
		private const int AllPropertyChoiceIndex = MaximumPropertyChoices;
		private int _targetProperty = AllPropertyChoiceIndex;
		private string _lastSelectSubPropertyPath;

		internal string DimensionNamePrefix { get; set; } = string.Empty;
		internal string DimensionNamePrefixShort { get; set; } = string.Empty;

		private static readonly string[] DimensionNames = { "X", "Y", "Z" };

		private const string PropertyNameX = nameof(DynamicsVector3.dynamicsX);
		private const string PropertyNameY = nameof(DynamicsVector3.dynamicsY);
		private const string PropertyNameZ = nameof(DynamicsVector3.dynamicsZ);
		private const string PropertyNameSettings = nameof(SecondOrderDynamics.settings);

		private const string PropertyNameModifyX = nameof(DynamicsVector3.modifyX);
		private const string PropertyNameModifyY = nameof(DynamicsVector3.modifyY);
		private const string PropertyNameModifyZ = nameof(DynamicsVector3.modifyZ);

		#region Overrides of DynamicsTransformComponentDrawer

		/// <inheritdoc />
		//protected override float HeightDivisor { get; } = 3.0f;

		/// <inheritdoc />
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			base.OnGUI(position, property, label);
		}

		protected override void OnUpdateLines(IMultidimensionalDynamicsProvider mdDynamics)
		{
			if (mdDynamics is not DynamicsVector3 dv3)
			{
				return;
			}

			UpdateLines(dv3);
		}

		protected override void OnRenderGraph(Rect position,
											IMultidimensionalDynamicsProvider mdDynamics,
											bool isExpanded
		)
		{
			if (mdDynamics is not DynamicsVector3 dv3)
			{
				return;
			}

			RenderGraph(position, dv3, isExpanded);
		}

		protected override void OnRenderInspector(Rect position, SerializedProperty property)
			=> RenderInspector(position, property);

		protected override float OnGetShownPropertyHeight(SerializedProperty property)
			=> GetShownPropertyHeight(property);

		#endregion

		internal void UpdateLines(DynamicsVector3 dv3)
		{
			UpdateLine(dv3.dynamicsX, dv3.modifyX, 0);
			UpdateLine(dv3.dynamicsY, dv3.modifyY, 1);
			UpdateLine(dv3.dynamicsZ, dv3.modifyZ, 2);
		}

		internal void RenderGraph(Rect position, DynamicsVector3 dv3, bool isExpanded)
		{
			if (isExpanded)
			{
				Renderer.FocusLines(_targetProperty == AllPropertyChoiceIndex
										? null
										: CreateLineKey(_targetProperty));

				Renderer.Render(position, GUIContent.none, isExpanded: true);
				return;
			}

			float horizontalSpacer = Constants.StandardHorizontalSpacing;
			Rect tabRect = position.EditorGUILineHeightTabs(dv3.DimensionCount, horizontalSpacer);

			for (int scan = 0; scan < dv3.DimensionCount; ++scan)
			{
				Renderer.SetSingleLineVisible(CreateLineKey(scan));
				Renderer.Render(tabRect, $"{DimensionNamePrefixShort}{DimensionNames[scan]}", isExpanded: false);
				tabRect.EditorGUINextTab(horizontalSpacer);
			}
		}

		internal void RenderInspector(Rect position, SerializedProperty property)
		{
			SerializedProperty propModX = property.FindPropertyRelative(PropertyNameModifyX);
			SerializedProperty propModY = property.FindPropertyRelative(PropertyNameModifyY);
			SerializedProperty propModZ = property.FindPropertyRelative(PropertyNameModifyZ);

			bool modX = propModX?.GetValue<bool>() ?? false;
			bool modY = propModY?.GetValue<bool>() ?? false;
			bool modZ = propModZ?.GetValue<bool>() ?? false;

			bool useToggles = propModX != null && propModY != null && propModZ != null;

			ToolbarToggleState allToggleState = Toolbar.ToToggleState(modX, modY, modZ);

			ToolbarChoice<string>[] choices =
			{
				new($"{DimensionNamePrefix}{DimensionNames[0]}", $"{PropertyNameX}.{PropertyNameSettings}", modX),
				new($"{DimensionNamePrefix}{DimensionNames[1]}", $"{PropertyNameY}.{PropertyNameSettings}", modY),
				new($"{DimensionNamePrefix}{DimensionNames[2]}", $"{PropertyNameZ}.{PropertyNameSettings}", modZ),
				new("All", $"{PropertyNameX}.{PropertyNameSettings}", allToggleState),
			};

			// toolbar for choosing dimension
			EditorGUI.BeginChangeCheck();
			bool wasToggle = false;

			Rect toolbarPosition = position.EditorGUILineHeight();

			if (useToggles)
			{
				(_targetProperty, wasToggle) =
					Toolbar.DrawWithToggles(toolbarPosition, _targetProperty, choices);
			}
			else
			{
				_targetProperty = Toolbar.Draw(toolbarPosition, _targetProperty, choices);
			}

			position.EditorGUINextLine();

			// did the user toggle a modify state?
			if (EditorGUI.EndChangeCheck() && wasToggle)
			{
				bool newModState = choices[_targetProperty].ToggleState == ToolbarToggleState.Toggled;
				switch (_targetProperty)
				{
					case 0:
						propModX.SetValue(newModState);
						break;
					case 1:
						propModY.SetValue(newModState);
						break;
					case 2:
						propModZ.SetValue(newModState);
						break;
					default:
						propModX.SetValue(newModState);
						propModY.SetValue(newModState);
						propModZ.SetValue(newModState);
						break;
				}

				property.serializedObject.ApplyModifiedProperties();
			}

			_lastSelectSubPropertyPath = choices[_targetProperty].Data;
			SerializedProperty prop = property.FindPropertyRelative(_lastSelectSubPropertyPath);

			// if we are doing 'all' dimensions, we'll the X
			// dimension to set all of them on changes.
			SerializedProperty all = null;
			if (_targetProperty == AllPropertyChoiceIndex)
			{
				all = prop.Copy();
				EditorGUI.BeginChangeCheck();
			}

			// draw the dimension's inspector
			OnGUIChildren(position, prop);

			// again, if we're doing all dimensions, we need to handle it:
			// first, apply the changes that were made,
			// then, update the other properties, which in this case will be Y and Z.
			if (all != null && EditorGUI.EndChangeCheck())
			{
				all.serializedObject.ApplyModifiedPropertiesWithoutUndo();
				object val = all.GetValue();

				for (int scan = 1; scan < DimensionNames.Length; ++scan)
				{
					SerializedProperty dimensionProperty = property.FindPropertyRelative(choices[scan].Data);
					dimensionProperty.boxedValue = val;
				}

				all.serializedObject.ApplyModifiedProperties();
			}
		}

		internal float GetShownPropertyHeight(SerializedProperty property)
		{
			SerializedProperty selectedProperty = property?.FindPropertyRelative(_lastSelectSubPropertyPath);

			float baseHeight = EditorGUIUtility.singleLineHeight;

			return selectedProperty == null
					   ? baseHeight
					   : baseHeight + EditorGUIUtility.standardVerticalSpacing
									+ GetChildPropertyFieldsHeight(selectedProperty);
		}
	}
}
