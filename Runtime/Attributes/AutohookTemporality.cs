using System;

using UnityEngine;

namespace cmdwtf.UnityTools
{
	/// <summary>
	/// Timing with which <see cref="AutohookAttribute"/> should
	/// decide when to attempt to hook up components.
	/// </summary>
	[Flags]
	public enum AutohookTemporality : uint
	{
		/// <summary>
		/// The <see cref="Component"/> should be found and attached while in the editor.
		/// </summary>
		Editor = 0x1,
		/// <summary>
		/// The <see cref="Component"/> should be found and attached on scene load.
		/// </summary>
		RuntimeSceneLoad = 0x2,
		/// <summary>
		/// The <see cref="Component"/> should be found when the object is created.
		/// N.B.: This will occur as the attribute itself is created.
		/// </summary>
		RuntimeOnDemand = 0x04,
		/// <summary>
		/// The <see cref="Component"/> should be found and attached whenever possible.
		/// </summary>
		All = 0xFFFF_FFFF,
	}
}
