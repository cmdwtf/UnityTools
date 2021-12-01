using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace cmdwtf.UnityTools
{
	#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoad]
	#endif // UNITY_EDITOR
	public static class GUITools
	{
		private static readonly Stack<Color> ColorStack;
		private static readonly Stack<bool> EnabledStack;

		static GUITools()
		{
			ColorStack = new Stack<Color>();
			EnabledStack = new Stack<bool>();
		}

		[RuntimeInitializeOnLoadMethod]
		private static void RuntimeInitialize()
		{
			ColorStack.Clear();
			EnabledStack.Clear();
		}

		public static void PushColor(Color newColor)
		{
			ColorStack.Push(GUI.color);
			GUI.color = newColor;
		}

		public static void PopColor()
		{
			if (ColorStack.Any())
			{
				GUI.color = ColorStack.Pop();
			}
		}

		public static void PushEnabled(bool newEnabled)
		{
			EnabledStack.Push(GUI.enabled);
			GUI.enabled = newEnabled;
		}

		public static void PopEnabled()
		{
			if (EnabledStack.Any())
			{
				GUI.enabled = EnabledStack.Pop();
			}
		}
	}
}
