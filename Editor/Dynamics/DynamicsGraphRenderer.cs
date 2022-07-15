using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using cmdwtf.UnityTools.Attributes;
using cmdwtf.UnityTools.Dynamics;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor.Dynamics
{
	internal class DynamicsGraphRenderer
	{
		private static readonly Color DefaultTargetColor = Color.yellow;
		private static readonly Color DefaultBackgroundColor = Graph.DefaultBackgroundColor;
		private static readonly Color DefaultFailureBackgroundColor = Constants.DarkRed;

		private const string LineKeyTarget = "__internalTargetKey";

		private readonly Graph _graphRenderer;

		private string _nominalGraphSubText = string.Empty;

		private readonly HashSet<LineMeta> _failedLines = new();

		private class LineMeta
		{
			public uint ContentHash { get; set; }
			public StabilityState Stability { get; set; }
			public bool IsUnstable => Stability != StabilityState.Stable;
		}

		public GUIStyle LabelStyle
		{
			get => _graphRenderer.LabelStyle;
			set => _graphRenderer.LabelStyle = value;
		}

		public bool DrawLabels
		{
			get => _graphRenderer.ShouldDrawLabels;
			set => _graphRenderer.ShouldDrawLabels = value;
		}

		public float FrameWidth
		{
			get => _graphRenderer.FrameThicknessPx;
			set => _graphRenderer.FrameThicknessPx = value;
		}

		public Color FrameColor
		{
			get => _graphRenderer.FrameColor;
			set => _graphRenderer.FrameColor = value;
		}

		public Color GridLinesColor
		{
			get => _graphRenderer.GridLinesColor;
			set => _graphRenderer.GridLinesColor = value;
		}

		public Color HorizontalAxisColor
		{
			get => _graphRenderer.HorizontalAxisColor;
			set => _graphRenderer.HorizontalAxisColor = value;
		}

		public Color VerticalAxisColor
		{
			get => _graphRenderer.VerticalAxisColor;
			set => _graphRenderer.VerticalAxisColor = value;
		}

		public bool DrawTargetLine { get; set; } = true;
		public Color TargetLineColor { get; set; } = DefaultTargetColor;
		public Color BackgroundColor { get; set; } = DefaultBackgroundColor;
		public Color BackgroundFailureColor { get; set; } = DefaultFailureBackgroundColor;

		public DynamicsGraphRenderer()
		{
			_graphRenderer = new()
			{
				HorizontalAxisUnits = "s", /* horizontal is time */
			};


			// set the bottom graph text handler
			_graphRenderer.GetHorizontalMidText = userData =>
				(userData is LineMeta lineMeta && _failedLines.Contains(lineMeta)
					 ? $" [Failure: {lineMeta.Stability}]"
					 : _nominalGraphSubText);
		}

		public void UpdateSimulationTargetLine(float targetValue, Color targetColor)
		{
			float[] targetSamples = {targetValue, targetValue};
			_graphRenderer.UpdateLine(LineKeyTarget, targetSamples, targetColor);
		}

		public bool DoesLineNeedUpdate(string lineKey, uint contentHash)
		{
			uint lineHash = GetLineMeta(lineKey).ContentHash;
			return lineHash == 0 || lineHash == contentHash;
		}

		public void UpdateDynamicsLine(DynamicsSimulationConfig config, string lineKey, Color? lineColor = null, uint contentHash = 0)
		{
			// update the graph's limits
			_graphRenderer.ExtendLogicalRanges(
				config.OriginSeconds,
				config.DurationSeconds,
				config.OriginValue,
				config.TargetValue);

			if (DrawTargetLine)
			{
				UpdateSimulationTargetLine(config.TargetValue, TargetLineColor);
			}

			// grab (or create) our meta instance
			LineMeta meta = GetLineMeta(lineKey);
			meta.ContentHash = contentHash;

			// grab a dynamics instance and simulate it
			(float[] values, StabilityState stability) = Simulate(config);
			meta.Stability = stability;

			_nominalGraphSubText = $"{config.Dynamics}; ΔT={config.StepDeltaTime}s";

			// update the line on the graph,
			_graphRenderer.UpdateLine(lineKey, values, lineColor, meta);

			// set the background color based on
			if (meta.IsUnstable)
			{
				_failedLines.Add(meta);
			}
			else
			{
				_failedLines.Remove(meta);
			}
		}

		private static (float[] Result, StabilityState Stability) Simulate(DynamicsSimulationConfig config)
		{
			float[] result = new float[config.SampleCount];

			// reset state with a temporary initial value
			config.Dynamics.PushState();
			config.Dynamics.ResetTemporarySim(config.OriginValue);

			// we will record if the system fails at any step of the simulation
			StabilityState stabilityState = StabilityState.Stable;

			// simulate each sample
			for (int scan = 0; scan < config.SampleCount; ++scan)
			{
				result[scan] = config.Dynamics.UpdateSim(config.StepDeltaTime, config.TargetValue);

				if (!config.Dynamics.IsStable)
				{
					stabilityState = config.Dynamics.StabilityState;
				}
			}

			// restore the state after simulating
			config.Dynamics.PopState();

			return (result, stabilityState);
		}

		public void RenderInto(Rect position)
		{
			_graphRenderer.BackgroundFillColor = _failedLines.Count > 0
													 ? BackgroundFailureColor
													 : BackgroundColor;
			_graphRenderer.Draw(position);
		}

		public void RenderCollapsedInto(Rect position)
		{
			bool previousDrawTarget = DrawTargetLine;
			bool previousLabels = DrawLabels;
			bool previousHorizontalAxis = _graphRenderer.ShouldDrawHorizontalAxis;
			bool previousVerticalAxis = _graphRenderer.ShouldDrawVerticalAxis;

			DrawTargetLine = false;
			DrawLabels = false;
			_graphRenderer.ShouldDrawHorizontalAxis = false;
			_graphRenderer.ShouldDrawVerticalAxis = false;

			RenderInto(position);

			DrawTargetLine = previousDrawTarget;
			DrawLabels = previousLabels;
			_graphRenderer.ShouldDrawHorizontalAxis = previousHorizontalAxis;
			_graphRenderer.ShouldDrawVerticalAxis = previousVerticalAxis;
		}

		public void Render(Rect position, string labelText, bool isExpanded)
			=> Render(position, new GUIContent(labelText), isExpanded);

		public void Render(Rect position, GUIContent label, bool isExpanded)
		{
			if (label != GUIContent.none)
			{
				float textWidth = LabelStyle.CalcSize(label).x;
				Rect labelRect = position.CollapseToLeft(textWidth + Constants.StandardHorizontalSpacing);
				GUI.Label(labelRect, label);
				position = position.CollapseToRight(position.width - textWidth);
			}

			if (isExpanded)
			{
				RenderInto(position);
			}
			else
			{
				RenderCollapsedInto(position);
			}
		}

		private LineMeta GetLineMeta(string lineKey)
		{
			LineMeta currentMeta = _graphRenderer.GetLineUserData<LineMeta>(lineKey);

			return currentMeta ?? new LineMeta();
		}

		public void SetAllLinesVisible(bool visible) => _graphRenderer.SetAllLinesVisible(visible);

		public void SetSingleLineVisible(string lineKey)
		{
			_graphRenderer.SetAllLinesVisible(false);
			_graphRenderer.SetLineVisible(lineKey, true);
		}

		public void FocusLines(params string[] lineKeys) => _graphRenderer.FocusLines(lineKeys);
	}
}
