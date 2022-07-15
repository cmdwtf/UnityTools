using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	internal class ToolbarChoice<T>
	{
		public string Text => Display.text;
		public GUIContent Display { get; }
		public T Data { get; }
		public ToolbarToggleState ToggleState { get; internal set; }

		public ToolbarChoice(T data, bool toggled)
			: this(data, toggled.ToToggleState())
		{ }

		public ToolbarChoice(string displayText, T data, bool toggled)
			: this(new GUIContent(displayText), data, toggled.ToToggleState())
		{ }

		public ToolbarChoice(GUIContent display, T data, bool toggled)
			: this(display, data, toggled.ToToggleState())
		{ }

		public ToolbarChoice(T data, ToolbarToggleState tts = ToolbarToggleState.Normal)
			: this(GUIContent.none, data, tts)
		{ }

		public ToolbarChoice(string displayText, T data, ToolbarToggleState tts = ToolbarToggleState.Normal)
			: this(new GUIContent(displayText), data, tts)
		{ }

		public ToolbarChoice(GUIContent display, T data, ToolbarToggleState tts = ToolbarToggleState.Normal)
		{
			Data = data;
			Display = display;
			ToggleState = tts;
		}
	}
}
