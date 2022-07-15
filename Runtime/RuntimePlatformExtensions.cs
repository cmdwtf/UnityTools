using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class RuntimePlatformExtensions
	{
		/// <summary>
		/// Returns true if the platform is aa Windows platform.
		/// </summary>
		/// <param name="rp">The platform to check.</param>
		/// <returns>True for Windows platforms, otherwise false.</returns>
		public static bool IsWindows(this RuntimePlatform rp)
		{
			return rp == RuntimePlatform.WindowsPlayer ||
				   rp == RuntimePlatform.WindowsEditor ||
#if !UNITY_5_4_OR_NEWER
				   rp == RuntimePlatform.WindowsWebPlayer ||
#endif // !UNITY_5_4_OR_NEWER
				   rp == RuntimePlatform.WSAPlayerX86 ||
				   rp == RuntimePlatform.WSAPlayerX64 ||
				   rp == RuntimePlatform.WSAPlayerARM;
		}

		/// <summary>
		/// Returns true if the platform is an MacOS platform.
		/// </summary>
		/// <param name="rp">The platform to check.</param>
		/// <returns>True for MacOS platforms, otherwise false.</returns>
		public static bool IsMacOS(this RuntimePlatform rp)
		{
			return rp == RuntimePlatform.OSXEditor ||
				   rp == RuntimePlatform.OSXPlayer
#if !UNITY_5_4_OR_NEWER
				   || rp == RuntimePlatform.OSXWebPlayer
				   || rp == RuntimePlatform.OSXDashboardPlayer
#endif // !UNITY_5_4_OR_NEWER
				;
		}

		/// <summary>
		/// Returns true if the platform is a Linux platform.
		/// N.B.: Even though Android is technically derived from Linux,
		/// it will not return true from this function. Use
		/// <see cref="IsAndroid"/> instead.
		/// </summary>
		/// <param name="rp">The platform to check.</param>
		/// <returns>True for Linux platforms, otherwise false.</returns>
		public static bool IsLinux(this RuntimePlatform rp)
			=> rp == RuntimePlatform.LinuxPlayer ||
			   rp == RuntimePlatform.LinuxEditor;

		/// <summary>
		/// Returns true if the platform is an iOS platform. (Incl. tvOS)
		/// </summary>
		/// <param name="rp">The platform to check.</param>
		/// <returns>True for iOS platforms, otherwise false.</returns>
		public static bool IsIOS(this RuntimePlatform rp)
			=> rp == RuntimePlatform.IPhonePlayer ||
			   rp == RuntimePlatform.tvOS;

		/// <summary>
		/// Returns true if the platform is an Android platform.
		/// </summary>
		/// <param name="rp">The platform to check.</param>
		/// <returns>True for Android platforms, otherwise false.</returns>
		public static bool IsAndroid(this RuntimePlatform rp)
			=> rp == RuntimePlatform.Android;

		/// <summary>
		/// Returns true if the platform is a Desktop platform.
		/// </summary>
		/// <param name="rp">The platform to check.</param>
		/// <returns>True for Desktop platforms, otherwise false.</returns>
		public static bool IsDesktop(this RuntimePlatform rp)
			=> rp.IsWindows() ||
			   rp.IsMacOS() ||
			   rp.IsLinux();

		/// <summary>
		/// Returns true if the platform is a Mobile platform.
		/// N.B.: This will include TV platforms.
		/// </summary>
		/// <param name="rp">The platform to check.</param>
		/// <returns>True for Desktop platforms, otherwise false.</returns>
		public static bool IsMobileOS(this RuntimePlatform rp)
		{
			return rp.IsIOS() ||
				   rp.IsAndroid() ||
				   rp.IsTV()
#if !UNITY_5_3_OR_NEWER
				   || rp == RuntimePlatform.WP8Player
#endif // !UNITY_5_3_OR_NEWER
#if !UNITY_5_4_OR_NEWER
				   || rp == RuntimePlatform.BB10Player
				   || rp == RuntimePlatform.BlackBerryPlayer
#endif // !UNITY_5_4_OR_NEWER
				;
		}

		public static bool IsWeb(this RuntimePlatform rp)
		{
			return rp == RuntimePlatform.WebGLPlayer
#if !UNITY_5_3_OR_NEWER
				   || rp == RuntimePlatform.FlashPlayer
#endif // !UNITY_5_0_OR_NEWER
#if !UNITY_5_4_OR_NEWER
				   || rp == RuntimePlatform.WindowsWebPlayer
				   || rp == RuntimePlatform.BlackBerryPlayer
#endif // !UNITY_5_4_OR_NEWER
				;
		}

		/// <summary>
		/// Returns true if the platform is a Console platform.
		/// N.B.: This will return true for TV platforms too (e.g.: tvOS),
		/// as TV platforms are very similar to traditional 'consoles'.
		/// </summary>
		/// <param name="rp">The platform to check.</param>
		/// <returns>True for Console platforms, otherwise false.</returns>
		public static bool IsConsole(this RuntimePlatform rp)
		{
			return rp.IsTV() ||
#if !UNITY_5_4_OR_NEWER
				rp == RuntimePlatform.PS3 ||
				rp == RuntimePlatform.XBOX360 ||
#endif // !UNITY_5_4_OR_NEWER
				rp == RuntimePlatform.PS4 ||
#if !UNITY_5_3_OR_NEWER
				rp == RuntimePlatform.PSM ||
#endif // !UNITY_5_3_OR_NEWER
				rp == RuntimePlatform.XboxOne ||
#if !UNITY_2018_1_OR_NEWER
				rp == RuntimePlatform.WiiU ||
#endif // !UNITY_2018_1_OR_NEWER
				rp == RuntimePlatform.Switch ||
				rp == RuntimePlatform.GameCoreXboxSeries ||
				rp == RuntimePlatform.GameCoreXboxOne ||
				rp == RuntimePlatform.PS5;
		}


		/// <summary>
		/// Returns true if the platform is a TV platform.
		/// </summary>
		/// <param name="rp">The platform to check.</param>
		/// <returns>True for TV platforms, otherwise false.</returns>
		public static bool IsTV(this RuntimePlatform rp)
		{
			return
#if !UNITY_2017_3_OR_NEWER
				rp == RuntimePlatform.TizenPlayer ||
				rp == RuntimePlatform.SamsungTVPlayer ||
#endif // UNITY_2017_3_OR_NEWER
				rp == RuntimePlatform.tvOS;
		}

		public static bool IsCloud(this RuntimePlatform rp)
			=> rp == RuntimePlatform.Stadia ||
#if UNITY_2022_1_OR_NEWER
			   rp == RuntimePlatform.LinuxPlayer;
#else
			   rp == RuntimePlatform.CloudRendering;
#endif // UNITY_2022_1_OR_NEWER
	}
}
