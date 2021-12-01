using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class Colors
	{
		// Colors based on default bootstrap colors

		public static Color primary { get; } = ColorTools.FromBytes(0, 123, 255);
		public static Color secondary { get; } = ColorTools.FromBytes(108, 117, 125);
		public static Color success { get; } = ColorTools.FromBytes(40, 167, 69);
		public static Color danger { get; } = ColorTools.FromBytes(220, 53, 69);
		public static Color warning { get; } = ColorTools.FromBytes(255, 193, 7);
		public static Color info { get; } = ColorTools.FromBytes(23, 162, 184);
		public static Color light { get; } = ColorTools.FromBytes(248, 249, 250);
		public static Color dark { get; } = ColorTools.FromBytes(52, 58, 64);
		public static Color muted { get; } = ColorTools.FromBytes(108, 117, 125);
		public static Color white { get; } = Color.white;
	}
}
