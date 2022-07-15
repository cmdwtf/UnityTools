using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	internal static class Constants
	{
		public static float StandardHorizontalSpacing { get; } = EditorGUIUtility.standardVerticalSpacing * 2;

		public static float StandardLineHeight { get; } =
			EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

		public const string RefreshSymbol = "↻";

		public static readonly Color NearlyBlack = new(0.0625f, 0.0625f, 0.0625f, 1f);
		public static readonly Color VeryDarkGray = new(0.125f, 0.125f, 0.125f, 1f);
		public static readonly Color DarkerGray = new(0.2f, 0.2f, 0.2f, 1f);
		public static readonly Color DarkGray = new(0.25f, 0.25f, 0.25f, 1f);

		public static readonly Color DarkRed = new(0.33f, 0f, 0f, 1f);
	}
}
