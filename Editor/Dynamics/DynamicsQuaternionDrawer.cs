using System;

using cmdwtf.UnityTools.Dynamics;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor.Dynamics
{
	[UnityEditor.CustomPropertyDrawer(typeof(DynamicsQuaternion), useForChildren: true)]
	public class DynamicsQuaternionDrawer : DynamicsTransformComponentDrawer
	{
		private const int QuaternionFixedDynamicsLineIndex = 3;

		private readonly DynamicsVector3Drawer _forwardDrawer;
		private readonly DynamicsVector3Drawer _upwardsDrawer;

		private float _forwardPropertyHeight = 0;
		private float _upwardsPropertyHeight = 0;
		private float _shownPropertyHeight = 0;

		private Action<Rect> _renderFullGraph;
		private Rect _graphStartPosition;

		private static readonly string[] DimensionPropertyNames =
		{
			nameof(DynamicsQuaternion.dynamicsX),
			nameof(DynamicsQuaternion.dynamicsY),
			nameof(DynamicsQuaternion.dynamicsZ),
			nameof(DynamicsQuaternion.dynamicsW),
		};

		private const string PropertyNameSettings = nameof(SecondOrderDynamics.settings);

		private const string PropertyNameNormalizeSign = nameof(DynamicsQuaternion.normalizeSign);

		public DynamicsQuaternionDrawer()
		{
			_forwardDrawer = new DynamicsVector3Drawer
			{
				DimensionNamePrefix = "Forwards ",
				DimensionNamePrefixShort = "F",
			};

			_upwardsDrawer = new DynamicsVector3Drawer
			{
				DimensionNamePrefix = "Upwards ",
				DimensionNamePrefixShort = "U",
				LineColorIndexOffset = 3,
			};
		}

		#region Overrides of DynamicsTransformComponentDrawer

		/// <inheritdoc />
		protected override void OnUpdateLines(IMultidimensionalDynamicsProvider mdDynamics)
		{
			if (mdDynamics is not DynamicsQuaternion dq)
			{
				return;
			}

			switch (dq.rotationType)
			{
				case DynamicsQuaternionRotationType.QuaternionComponents:
					// in quaternion mode, we will just use a single axis to represent all of them,
					// because trying to conceptualize how the imaginary parts of the q will affect
					// the result is mind-waffling.
					UpdateLine(dq.dynamicsW, true, QuaternionFixedDynamicsLineIndex);
					break;
				case DynamicsQuaternionRotationType.ForwardAndUpward:
					_forwardDrawer.UpdateLines(dq.dynamicsForward);
					_upwardsDrawer.UpdateLines(dq.dynamicsUpwards);
					break;
			}
		}

		/// <inheritdoc />
		protected override void OnRenderGraph(Rect position, IMultidimensionalDynamicsProvider mdDynamics, bool isExpanded)
		{
			if (mdDynamics is not DynamicsQuaternion dq)
			{
				return;
			}

			_graphStartPosition = position;

			if (dq.rotationType == DynamicsQuaternionRotationType.QuaternionComponents)
			{
				if (isExpanded)
				{
					// don't render the graph now -- it's delayed until after we render the mode.
					_renderFullGraph = r =>
					{
						_renderer.SetAllLinesVisible(true);
						_renderer.Render(r, GUIContent.none, isExpanded: true);
					};
				}
				else
				{
					_renderer.SetAllLinesVisible(true);
					_renderer.Render(position, "Rotation", isExpanded: false);
				}
			}
			else if (dq.rotationType == DynamicsQuaternionRotationType.ForwardAndUpward)
			{
				if (isExpanded)
				{
					// don't render the graph now -- it's delayed until after we render the mode.
					_renderFullGraph = r =>
					{
						_forwardDrawer.RenderGraph(r, dq.dynamicsForward, isExpanded: true);
						r.y += _forwardPropertyHeight + EditorGUIUtility.standardVerticalSpacing;
						_upwardsDrawer.RenderGraph(r, dq.dynamicsUpwards, isExpanded: true);
					};
				}
				else
				{
					float horizontalSpacer = Constants.StandardHorizontalSpacing;
					Rect tabRect = position.EditorGUILineHeightTabs(2, horizontalSpacer);

					_forwardDrawer.RenderGraph(tabRect, dq.dynamicsForward, isExpanded: false);
					tabRect.EditorGUINextTab(horizontalSpacer);

					_upwardsDrawer.RenderGraph(tabRect, dq.dynamicsUpwards, isExpanded: false);
				}
			}
		}

		/// <inheritdoc />
		protected override void OnRenderInspector(Rect position, SerializedProperty property)
		{
			Rect inspectorStartPosition = position;
			position = _graphStartPosition;

			SerializedProperty typeProperty = property.FindPropertyRelative(nameof(DynamicsQuaternion.rotationType));
			GUIContent typeLabel = new(typeProperty.displayName, typeProperty.tooltip);
			EditorGUI.PropertyField(position.EditorGUILineHeight(), typeProperty, typeLabel);

			float newGraphStartY = _graphStartPosition.y +
						 EditorGUI.GetPropertyHeight(typeProperty) +
						 EditorGUIUtility.standardVerticalSpacing;

			position.y = newGraphStartY;

			DynamicsQuaternionRotationType type = GetRotationType(property);
			switch (type)
			{
				case DynamicsQuaternionRotationType.QuaternionComponents:
					DoQuaternionDynamicsInspector(position, property);
					position.y += Constants.StandardLineHeight;
					position.height = _graphStartPosition.height;
					_renderFullGraph?.Invoke(position);
					position.y += position.height + DisplayHeightGraphPaddingPx;
					DoSingleQuaternionInspector(position, property);
					break;
				case DynamicsQuaternionRotationType.ForwardAndUpward:
					position.y = newGraphStartY;
					_renderFullGraph?.Invoke(position);
					position.y = inspectorStartPosition.y + DisplayHeightGraphPaddingPx;
					_forwardDrawer.RenderInspector(position, property);
					position.y += _forwardPropertyHeight + EditorGUIUtility.standardVerticalSpacing;
					_upwardsDrawer.RenderInspector(position, property);
					break;
			}
		}

		/// <inheritdoc />
		protected override float OnGetShownPropertyHeight(SerializedProperty property)
		{
			DynamicsQuaternionRotationType type = GetRotationType(property);

			float baseHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			SerializedProperty singleDimensionProperty = property.FindPropertyRelative($"{DimensionPropertyNames[0]}.{PropertyNameSettings}");
			float singleDimensionEditHeight = GetChildPropertyFieldsHeight(singleDimensionProperty) + (Constants.StandardLineHeight * 1);

			_forwardPropertyHeight = _forwardDrawer.GetPropertyHeight(property, GUIContent.none);
			_upwardsPropertyHeight = _upwardsDrawer.GetPropertyHeight(property, GUIContent.none);

			_shownPropertyHeight = type switch
			{
				DynamicsQuaternionRotationType.QuaternionComponents => baseHeight + singleDimensionEditHeight,
				DynamicsQuaternionRotationType.ForwardAndUpward =>	_forwardPropertyHeight + _upwardsPropertyHeight +
																	DisplayHeightGraphPaddingPx +
																	EditorGUIUtility.standardVerticalSpacing,
				_ => _shownPropertyHeight,
			};

			GraphHeightMultiplier = type == DynamicsQuaternionRotationType.QuaternionComponents
										? 1f
										: 0f;

			return _shownPropertyHeight;
		}

		/// <inheritdoc />
		protected override void OnApplyPreset(IDynamicsPreset preset)
		{
			if (preset is not SecondOrderDynamicsPreset sodPreset)
			{
				return;
			}

			if (_currentDynamics is not DynamicsQuaternion dq)
			{
				return;
			}

			DynamicsQuaternionRotationType type = GetRotationType(_currentProperty);
			switch (type)
			{
				case DynamicsQuaternionRotationType.QuaternionComponents:
					dq.dynamicsX.ApplyPreset(sodPreset);
					dq.dynamicsY.ApplyPreset(sodPreset);
					dq.dynamicsZ.ApplyPreset(sodPreset);
					dq.dynamicsW.ApplyPreset(sodPreset);
					break;
				case DynamicsQuaternionRotationType.ForwardAndUpward:
					dq.dynamicsForward.dynamicsX.ApplyPreset(sodPreset);
					dq.dynamicsForward.dynamicsY.ApplyPreset(sodPreset);
					dq.dynamicsForward.dynamicsZ.ApplyPreset(sodPreset);
					dq.dynamicsUpwards.dynamicsX.ApplyPreset(sodPreset);
					dq.dynamicsUpwards.dynamicsY.ApplyPreset(sodPreset);
					dq.dynamicsUpwards.dynamicsZ.ApplyPreset(sodPreset);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			_currentProperty.serializedObject.ApplyModifiedProperties();
		}

		#endregion

		private DynamicsQuaternionRotationType GetRotationType(SerializedProperty property)
		{
			SerializedProperty rotationType = property.FindPropertyRelative(nameof(DynamicsQuaternion.rotationType));
			return rotationType.GetValue<DynamicsQuaternionRotationType>();
		}

		private void DoQuaternionDynamicsInspector(Rect position, SerializedProperty property)
			=> EditorGUI.PropertyField(position.EditorGUILineHeight(),
									   property.FindPropertyRelative(PropertyNameNormalizeSign));

		private void DoSingleQuaternionInspector(Rect position, SerializedProperty property)
		{
			SerializedProperty propertySettingsW = property.FindPropertyRelative($"{DimensionPropertyNames[0]}.{PropertyNameSettings}");

			// if we are doing 'all' dimensions, we'll modify the X
			// dimension and then propagate them to the Y, Z, and W.
			SerializedProperty all = propertySettingsW.Copy();
			EditorGUI.BeginChangeCheck();

			// draw the dimension's inspector
			OnGUIChildren(position, propertySettingsW);

			// only need to copy changes if changes actually got made!
			if (!EditorGUI.EndChangeCheck())
			{
				return;
			}

			// apply the changes the user made to this object
			all.serializedObject.ApplyModifiedProperties();

			// update the serialized version so we can get the fresh value
			all.serializedObject.Update();

			// get the boxed value to propagate to other properties
			object val = all.GetValue();

			// skipping 0 because we started there, find each other property and set those values,
			for (int scan = 1; scan < DimensionPropertyNames.Length; ++scan)
			{
				SerializedProperty dimensionProperty = property.FindPropertyRelative($"{DimensionPropertyNames[scan]}.{PropertyNameSettings}");
				dimensionProperty.SetValueNoUndoRecord(val);
			}

			property.serializedObject.Update();
		}
	}
}
