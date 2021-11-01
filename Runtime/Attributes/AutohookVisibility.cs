using System;

namespace cmdwtf.UnityTools.Attributes
{
	/// <summary>
	/// The visibility of the <see cref="AutohookAttribute"/>ed property in the editor.
	/// </summary>
	[Flags]
	public enum AutohookVisibility
	{
		/// <summary>
		/// The Default visibility, adjustable via autohook settings.
		/// </summary>
		Default = 0x0,
		/// <summary>
		/// The field should be visible and editable.
		/// </summary>
		Visible = 0x1,
		/// <summary>
		/// The field should be visible, but disabled to be non-editable.
		/// </summary>
		Disabled = 0x2,
		/// <summary>
		/// The field should not be shown.
		/// </summary>
		Hidden = 0x4,
	}
}
