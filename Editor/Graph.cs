using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	/// <summary>
	/// A very feature-light tool to draw simple graphs in the Unity editor.
	/// </summary>
	public class Graph
	{
		#region Constants

		internal static readonly Color DefaultLineColor = Color.cyan;
		internal static readonly Color DefaultFrameColor = Constants.VeryDarkGray;
		internal static readonly Color DefaultBackgroundColor = Constants.DarkGray;
		internal static readonly Color DefaultGridLinesColor = DefaultFrameColor;
		internal static readonly Color DefaultAxisColor = Color.white;

		private const float DefaultFrameThicknessPx = 1f;
		private const float DefaultLineThickness = 0f;
		private const float DefaultLineFocusedThickness = 1f;

		private static readonly int DisplayAxisLabelHeightPx = (int)EditorGUIUtility.singleLineHeight;
		private const int DisplayAxisLabelWidthPx = 64;

		private static readonly Vector2 DisplayAxisLabelValueOffsetPx = new(-12, -DisplayAxisLabelHeightPx / 2f);
		private static readonly Vector2 DisplayAxisLabelTimePxRight = new(-DisplayAxisLabelWidthPx, 0);

		private static readonly Vector2 DisplayAxisLabelSize = new(DisplayAxisLabelWidthPx, DisplayAxisLabelHeightPx);

		#endregion

		#region Public Properties
		public float FrameThicknessPx { get; set; } = DefaultFrameThicknessPx;
		public float LineThickness { get; set; } = DefaultLineThickness;
		public float LineFocusedThickness { get; set; } = DefaultLineFocusedThickness;
		public Color FrameColor { get; set; } = DefaultFrameColor;
		public Color BackgroundFillColor { get; set; } = DefaultBackgroundColor;
		public Color GridLinesColor { get; set; } = DefaultGridLinesColor;
		public Color HorizontalAxisColor { get; set; } = DefaultAxisColor;
		public Color VerticalAxisColor { get; set; } = DefaultAxisColor;

		public bool ShouldDrawFrame { get; set; } = true;
		public bool ShouldDrawGridLines { get; set; } = false;
		public bool ShouldDrawLines { get; set; } = true;
		public bool ShouldDrawLabels { get; set; } = true;
		public bool ShouldDrawBackground { get; set; } = true;
		public bool ShouldDrawHorizontalAxis { get; set; } = true;
		public bool ShouldDrawVerticalAxis { get; set; } = true;
		public string HorizontalAxisUnits { get; set; } = string.Empty;
		public string VerticalAxisUnits { get; set; } = string.Empty;
		public Func<object, string> GetHorizontalMidText { private get; set; }

		public GUIStyle LabelStyle { get; set; } = new GUIStyle("label");

		#endregion

		#region Private State

		private Rect _frameDrawingPosition;

		private Rect _drawingPosition;

		private bool _needsLayout;

		private readonly Dictionary<string, GraphLine> _lines = new();

		private float AllLinesMinimumValue { get; set; }
		private float AllLinesMaximumValue { get; set; }

		// the logical/expected dimensions of the data to graph
		private GraphDimensions _logicalDimensions;

		// UI pixel coords to draw graph & others from
		private Vector2 _graphBottomLeft;
		private Vector2 _graphBottomRight;
		private Vector2 _graphTopLeft;
		private Vector2 _graphLogicalBottomLeft;
		private Vector2 _graphLogicalBottomRight;
		private Vector2 _graphLogicalTopLeft;

		#endregion

		#region Public Interface

		public Graph()
			: this(0, 1, 0, 1)
		{

		}

		public Graph(float minX, float maxX, float minY, float maxY)
		{
			SetLogicalRanges(minX, maxX, minY, maxY);
		}

		public void SetLogicalRanges(float minX, float maxX, float minY, float maxY)
		{
			if (minX > maxX)
			{
				throw new ArgumentOutOfRangeException(nameof(minX),
													  $"{nameof(maxX)} should be greater than {nameof(minX)}.");
			}
			if (minY > maxY)
			{
				throw new ArgumentOutOfRangeException(nameof(minY),
													  $"{nameof(maxY)} should be greater than {nameof(minY)}.");
			}

			_logicalDimensions = new GraphDimensions()
			{
				MinimumX = minX, MaximumX = maxX,
				MinimumY = minY, MaximumY = maxY,
			};
		}

		public void ExtendLogicalRanges(float minX, float maxX, float minY, float maxY)
		{
			if (minX > maxX)
			{
				throw new ArgumentOutOfRangeException(nameof(minX),
													  $"{nameof(maxX)} should be greater than {nameof(minX)}.");
			}
			if (minY > maxY)
			{
				throw new ArgumentOutOfRangeException(nameof(minY),
													  $"{nameof(maxY)} should be greater than {nameof(minY)}.");
			}

			// extend the ranges if they are exceeded
			_logicalDimensions.MinimumX = Mathf.Min(_logicalDimensions.MinimumX, minX);
			_logicalDimensions.MaximumX = Mathf.Min(_logicalDimensions.MaximumX, maxX);
			_logicalDimensions.MinimumY = Mathf.Min(_logicalDimensions.MinimumY, minY);
			_logicalDimensions.MaximumY = Mathf.Min(_logicalDimensions.MaximumY, maxY);
		}


		public T GetLineUserData<T>(string lineKey)
			=> _lines.TryGetValue(lineKey, out GraphLine line)
				? (T)line.UserData
				: default;

		public void UpdateLine(string lineKey, float[] ySamples, Color? lineColor = null, object userData = null)
		{
			if (!_lines.TryGetValue(lineKey, out GraphLine line))
			{
				line = new GraphLine();
				_lines.Add(lineKey, line);
			}

			line.UIPoints = new Vector3[ySamples.Length];
			line.Visible = true;
			line.UserData = userData;
			line.Color = lineColor ?? DefaultLineColor;
			line.SampleMinimum = Mathf.Min(_logicalDimensions.MinimumY, Mathf.Min(ySamples));
			line.SampleMaximum = Mathf.Max(_logicalDimensions.MaximumY, Mathf.Max(ySamples));


			// copy the samples to the line data, we will process them next time we layout.
			line.OriginalSamples = new float[ySamples.Length];
			Array.Copy(ySamples, line.OriginalSamples, ySamples.Length);

			_lines[lineKey] = line;

			UpdateValueExtremes();

			_needsLayout = true;
		}

		public bool RemoveLine(string lineKey) => _lines.Remove(lineKey);

		public void Draw(Rect position)
		{
			// see if our position changed requiring a new layout.
			if (_frameDrawingPosition != position)
			{
				_needsLayout = true;
			}

			_frameDrawingPosition = position;
			_drawingPosition = position.Shrink(FrameThicknessPx * 2);

			if (_needsLayout)
			{
				PerformLayout();
			}

			Handles.BeginGUI();

			// hold onto original handle color
			Color originalHandleColor = Handles.color;

			if (ShouldDrawFrame)
			{
				DrawFrame();
			}

			if (ShouldDrawBackground)
			{
				DrawBackground();
			}

			if (ShouldDrawGridLines)
			{
				DrawGridLines();
			}

			if (ShouldDrawHorizontalAxis)
			{
				Handles.color = HorizontalAxisColor;
				Handles.DrawLine(_graphLogicalBottomLeft, _graphLogicalBottomRight);
			}

			if (ShouldDrawVerticalAxis)
			{
				Handles.color = VerticalAxisColor;
				Handles.DrawLine(_graphBottomLeft, _graphTopLeft);
			}

			if (ShouldDrawLines)
			{
				DrawLines(focused: false);
				DrawLines(focused: true);
			}

			// reset the handles' color back to not disturb further drawing
			Handles.color = originalHandleColor;
			Handles.EndGUI();

			// draw our labels last so they render on top of the graph drawn areas
			if (ShouldDrawLabels)
			{
				DrawLabels();
			}
		}

		public void SetAllLinesVisible(bool visible)
		{
			foreach (GraphLine line in _lines.Values)
			{
				line.Focused = visible;
				line.Visible = visible;
			}
		}

		public void SetLineVisible(string lineKey, bool visible)
		{
			if (!_lines.TryGetValue(lineKey, out GraphLine line))
			{
				return;
			}

			line.Focused = visible;
			line.Visible = visible;
		}

		public void FocusLines(params string[] lineKeys)
		{
			foreach (KeyValuePair<string, GraphLine> kvp in _lines)
			{
				kvp.Value.Focused = lineKeys == null || lineKeys.Contains(kvp.Key);
			}
		}

		#endregion

		#region Private Methods

		private Vector2 SampleToUIPoint(float xSample, float ySample)
		{
			float valueDelta = (AllLinesMaximumValue - AllLinesMinimumValue);
			float xResult = Mathf.Lerp(_drawingPosition.xMin, _drawingPosition.xMax, xSample);
			float yResult = Mathf.Lerp(_drawingPosition.yMax, _drawingPosition.yMin, (ySample - AllLinesMinimumValue) / valueDelta);
			return new Vector2(xResult, yResult);
		}

		private void PerformLayout()
		{
			UpdateValueExtremes();

			foreach (GraphLine line in _lines.Values)
			{
				int scanMax = line.OriginalSamples.Length - 1;

				for (int scan = 0; scan < line.OriginalSamples.Length; ++scan)
				{
					float percent = scan / (float)scanMax;
					float xSample = Mathf.Lerp(_logicalDimensions.MinimumX, _logicalDimensions.MaximumX, percent);

					// convert graph x/y to gui positions and store them.
					line.UIPoints[scan] = SampleToUIPoint(xSample, line.OriginalSamples[scan]);
				}
			}

			// calculate pixel coords based on the logical graph size
			_graphLogicalBottomLeft = SampleToUIPoint(_logicalDimensions.MinimumX, _logicalDimensions.MinimumY);
			_graphLogicalBottomRight = SampleToUIPoint(_logicalDimensions.MaximumX, _logicalDimensions.MinimumY);
			_graphLogicalTopLeft = SampleToUIPoint(_logicalDimensions.MinimumX, _logicalDimensions.MaximumY);

			// calculate pixel cords based on the actual data
			_graphBottomLeft = SampleToUIPoint(_logicalDimensions.MinimumX, AllLinesMinimumValue);
			_graphBottomRight = SampleToUIPoint(_logicalDimensions.MaximumX, AllLinesMinimumValue);
			_graphTopLeft = SampleToUIPoint(_logicalDimensions.MinimumX, AllLinesMaximumValue);

			_needsLayout = false;
		}

		private void UpdateValueExtremes()
		{
			if (_lines.Count == 0)
			{
				AllLinesMaximumValue = _logicalDimensions.MinimumY;
				AllLinesMaximumValue = _logicalDimensions.MaximumY;
				return;
			}

			float[] minimumValues = _lines.Values.Select(l => l.SampleMinimum).ToArray();
			float[] maximumValues = _lines.Values.Select(l => l.SampleMaximum).ToArray();

			AllLinesMinimumValue = MathF.Min(_logicalDimensions.MinimumY,Mathf.Min(minimumValues));
			AllLinesMaximumValue = Mathf.Max(_logicalDimensions.MaximumY, Mathf.Max(maximumValues));
		}

		private void DrawFrame()
			=> EditorGUI.DrawRect(_frameDrawingPosition, FrameColor);

		private void DrawBackground()
			=> EditorGUI.DrawRect(_drawingPosition, BackgroundFillColor);

		// #nyi gridlines
		private void DrawGridLines() =>
			GUI.Label(_drawingPosition, "Gridlines Not Yet Implemented");

		private void DrawLines(bool focused)
		{
			// draw all lines
			foreach (GraphLine line in _lines.Values)
			{
				if (!line.Visible)
				{
					continue;
				}

				if (line.Focused != focused)
				{
					continue;
				}

				float alpha = line.Focused ? 1f : 0.5f;
				Handles.color = line.Color.WithAlpha(alpha);

				for (int scan = 1; scan < line.UIPoints.Length; ++scan)
				{
					Vector3 lineBegin = line.UIPoints[scan - 1];
					Vector3 lineEnd = line.UIPoints[scan];
					Handles.DrawLine(lineBegin, lineEnd, focused ? LineFocusedThickness : LineThickness);
				}
			}
		}

		private void DrawLabels()
		{
			Vector2 baseLineTextLocation = _graphBottomLeft;
			Vector2 baseLineTextLocationRight = _graphBottomRight;


			GUIStyle midTimeLabelStyle = new(LabelStyle) { alignment = TextAnchor.UpperCenter };
			GUIStyle maxTimeLabelStyle = new(LabelStyle) { alignment = TextAnchor.UpperRight };

			// ui positions to draw axis labels at
			var minYPos = new Rect(_graphLogicalBottomLeft + DisplayAxisLabelValueOffsetPx, DisplayAxisLabelSize);
			var targetYPos = new Rect(_graphLogicalTopLeft + DisplayAxisLabelValueOffsetPx, DisplayAxisLabelSize);
			var minXPos = new Rect(baseLineTextLocation, DisplayAxisLabelSize);
			var midXPos = new Rect(baseLineTextLocation, new Vector2(_drawingPosition.width, DisplayAxisLabelHeightPx));
			var maxXPos = new Rect(baseLineTextLocationRight + DisplayAxisLabelTimePxRight, DisplayAxisLabelSize);

			// draw x axis (horizontal) labels
			GUI.Label(minXPos, $"{_logicalDimensions.MinimumX}{HorizontalAxisUnits}", LabelStyle);
			GUI.Label(maxXPos, $"{_logicalDimensions.MaximumX}{HorizontalAxisUnits}", maxTimeLabelStyle);

			// draw the bottom middle text, if the user set the callback.
			if (GetHorizontalMidText != null)
			{
				GraphLine focusedLine = _lines.Values.FirstOrDefault(v => v.Focused);
				GUI.Label(midXPos, GetHorizontalMidText.Invoke(focusedLine?.UserData), midTimeLabelStyle);
			}

			// draw y axis (vertical) labels
			GUI.Label(minYPos, $"{_logicalDimensions.MinimumY}{VerticalAxisUnits}", LabelStyle);
			GUI.Label(targetYPos, $"{_logicalDimensions.MaximumY}{VerticalAxisUnits}", LabelStyle);

		}

		#endregion
	}
}
