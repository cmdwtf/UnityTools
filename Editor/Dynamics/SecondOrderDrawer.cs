using cmdwtf.UnityTools.Attributes;
using cmdwtf.UnityTools.Dynamics;

using UnityEngine;

using UnityEditor;

namespace cmdwtf.UnityTools.Editor.Dynamics
{
	[UnityEditor.CustomPropertyDrawer(typeof(SecondOrderDynamics))]
	public class SecondOrderDrawer : CustomPropertyDrawer
	{
		private const int DisplayHeightGraphLines = 6;
		private static readonly float DisplayHeightGraphPx = (DisplayHeightGraphLines * EditorGUIUtility.singleLineHeight) +
															 (DisplayHeightGraphLines * EditorGUIUtility.standardVerticalSpacing);

		private static readonly float DisplayHeightGraphPaddingPx = EditorGUIUtility.singleLineHeight;

		private static readonly GUIContent RefreshContent = new(Constants.RefreshSymbol, "Resample random inputs and re-draw preview.");
		private static readonly GUIStyle ButtonStyle = new(GUI.skin.button);

		private readonly DynamicsGraphRenderer _renderer;

		public SecondOrderDrawer()
		{
			DelegatedSubPropertyName = nameof(SecondOrderDynamics.settings);

			_renderer = new DynamicsGraphRenderer();
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			bool propertyModified = DelegatedPropertyField(position, ref property, label);
			float yOffset = GetDelegatedPropertyHeight(property, label, true);

			// grab our instance
			SecondOrderDynamics dynamics = property.GetValue<SecondOrderDynamics>();

			// decide if we're drawing big graph mode or preview based on our expansion state.
			if (property.isExpanded)
			{
				// move position down below the above inspector output.
				position.height = DisplayHeightGraphPx;
				position.Space(yOffset + EditorGUIUtility.standardVerticalSpacing);
			}
			else
			{
				position = position.EditorGUILineHeight().EditorGUIFieldWidth();
			}

			// give the user a 'refresh' button if they're using random sampling.
			if (property.isExpanded && dynamics.settings.inputSampleMode is SamplingMode.Random)
			{
				float resampleWidth = ButtonStyle.CalcSize(RefreshContent).x;
				if (GUI.Button(position.EditorGUILineHeight().CollapseToRight(resampleWidth), RefreshContent, ButtonStyle))
				{
					propertyModified = true;
				}
			}

			// we only need to simulate if the property content changed, otherwise we'll save the cycles and just draw the last data.
			if (propertyModified || _renderer.DoesLineNeedUpdate(property.propertyPath, property.contentHash))
			{
				// update graph
				DynamicsSimulationConfig config = new (dynamics);

				_renderer.UpdateDynamicsLine(config,
											 property.propertyPath,
											 GetCustomAttribute<DynamicsLineColorAttribute>()?.color,
											 property.contentHash);

			}

			// draw our graph!
			_renderer.Render(position, GUIContent.none, property.isExpanded);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			float baseHeight = GetDelegatedPropertyHeight(property, label, property.isExpanded);

			float graphHeight = DisplayHeightGraphPx + DisplayHeightGraphPaddingPx;

			// don't count all the graph area if we're collapsed.
			if (!property.isExpanded)
			{
				graphHeight = 0;
			}

			return baseHeight + graphHeight;
		}

		protected override int GetPropertyLineCount(SerializedProperty property, GUIContent label)
		{
			float height = GetPropertyHeight(property, label);
			PropertyLineCount = (int)(height / Constants.StandardLineHeight);
			return PropertyLineCount;
		}
	}
}
