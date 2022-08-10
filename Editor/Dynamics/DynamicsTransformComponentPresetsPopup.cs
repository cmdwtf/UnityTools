using System;
using System.Collections.Generic;
using System.Linq;

using cmdwtf.UnityTools.Dynamics;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor.Dynamics
{
	internal class DynamicsTransformComponentPresetsPopup : CustomPopupWindowContent<DynamicsTransformComponentPresetsPopup>
	{
		private static ValueRange Fcy(float v) => new ValueRange(v, v, SecondOrderSettings.DefaultFMinimum, SecondOrderSettings.DefaultFMaximum);
		private static ValueRange Damp(float v)	=> new(v, v, SecondOrderSettings.DefaultZMinimum, SecondOrderSettings.DefaultZMaximum);
		private static ValueRange Resp(float v)	=> new(v, v, SecondOrderSettings.DefaultRMinimum, SecondOrderSettings.DefaultRMaximum);

		private readonly SecondOrderDynamicsPreset[] _presets = {
			new("Smooth Damp", Fcy(3), Damp(1), Resp(0.5f)),
			new("Smooth Damp Fast", Fcy(3), Damp(1), Resp(1)),
			new("Smooth Damp Slow", Fcy(3), Damp(1), Resp(0)),
			new("Overdamped", Fcy(3), Damp(2), Resp(1)),
			new("Massive Overshoot", Fcy(3), Damp(1), Resp(5)),
			new("Massive Undershoot", Fcy(3), Damp(1), Resp(-5)),
			new("Small Wobble", Fcy(10), Damp(0.1f), Resp(1)),
			new("Large Wobble", Fcy(3), Damp(0.1f), Resp(1)),
		};

		private const int ChoicesPerRow = 4;
		private const int PreviewTextureWidthPx = 128;
		private const int PreviewTextureWidthRatio = 2;
		private const int PreviewTextureHeightPx = PreviewTextureWidthPx / PreviewTextureWidthRatio;

		private static readonly DynamicsGraphRenderer Renderer = new();
		private readonly Dictionary<SecondOrderDynamicsPreset, GUIContent> _presetChoices = new();
		private readonly ISimulatableDynamicsSystem _dynamics;

		private static readonly Color PreviewBackgroundColor = Constants.DarkGray;
		private static readonly Color PreviewLineColor = Color.white;

		public event Action<IDynamicsPreset> PresetSelected;

		public bool CloseOnSelection { get; set; } = true;

		public DynamicsTransformComponentPresetsPopup(IDynamicsSystem dynamics)
			: base("Presets")
		{
			Width = PreviewTextureWidthPx * (ChoicesPerRow * 1.15f);
			_dynamics = dynamics as ISimulatableDynamicsSystem;
			ShowAcceptButton = false;
		}

		#region Overrides of CustomPopupWindowContent<DynamicsTransformComponentPresetsPopup>

		/// <inheritdoc />
		protected override void ShouldDrawGUI(Rect position)
		{
			GUIStyle style = new(GUI.skin.button)
			{
				imagePosition = ImagePosition.ImageAbove,
			};

			GUIContent[] choices = _presets.Select(GetPresetChoice).ToArray();
			int userChoice = GUILayout.SelectionGrid(-1, choices, ChoicesPerRow, style);

			if (userChoice != -1)
			{
				PresetSelected?.Invoke(_presets[userChoice]);

				if (CloseOnSelection)
				{
					editorWindow.Close();
				}
			}
		}

		#endregion

		private GUIContent GetPresetChoice(SecondOrderDynamicsPreset preset)
		{
			if (_presetChoices.ContainsKey(preset))
			{
				return _presetChoices[preset];
			}

			GUIContent newChoice = new(preset.Name, preset.Name);

			if (_dynamics != null)
			{
				Texture2D previewImage = new(PreviewTextureWidthPx, PreviewTextureHeightPx, TextureFormat.RGBA32,
											 mipChain: false);
				Rect previewRect = new(0, 0, PreviewTextureWidthPx, PreviewTextureHeightPx);

				DynamicsSimulationConfig cfg = new(
					_dynamics,
					sampleCount: PreviewTextureWidthPx,
					overridePreset: preset
					);

				const string lineKey = "preview";
				Renderer.UpdateDynamicsLine(cfg, lineKey);
				Renderer.UpdateDrawingRect(previewRect);
				previewImage.Fill(PreviewBackgroundColor);
				DrawLine(previewImage, DynamicsGraphRenderer.LineKeyTarget, DynamicsGraphRenderer.DefaultTargetColor);
				DrawLine(previewImage, lineKey, PreviewLineColor);

				previewImage.Apply(updateMipmaps: false, makeNoLongerReadable: true);

				newChoice.image = previewImage;
			}

			_presetChoices[preset] = newChoice;

			return newChoice;
		}

		private static void DrawLine(Texture2D texture, string lineKey, Color lineColor)
		{
			Vector3[] points = Renderer.GetLineUIPoints(lineKey).ToArray();

			float textureHeight = texture.height;

			for (int scan = 1; scan < points.Length; ++scan)
			{
				Vector3 lineBegin = points[scan - 1];
				Vector3 lineEnd = points[scan];

				// invert the y, because our texture coords are from top left, not bottom left.
				lineBegin.y = textureHeight - lineBegin.y;
				lineEnd.y = textureHeight - lineEnd.y;

				texture.DrawLine(lineBegin, lineEnd, lineColor);
			}
		}
	}
}
