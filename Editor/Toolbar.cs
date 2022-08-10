using System;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	internal static class Toolbar
	{
		private static bool _stylesCreated;
		private static GUIContent _toggleOn;
		private static GUIContent _toggleNormal;
		private static GUIContent _toggleMixed;
		private static GUIStyle _buttonStyle;

		private static void EnsureStyles()
		{
			if (_stylesCreated)
			{
				return;
			}

			_stylesCreated = true;
			_toggleNormal = EditorGUIUtility.IconContent("ShurikenToggleNormal");
			_toggleOn = EditorGUIUtility.IconContent("ShurikenToggleNormalOn");
			_toggleMixed = EditorGUIUtility.IconContent("ShurikenToggleMixed");
			_buttonStyle = new GUIStyle(GUI.skin.button);
		}

		private static GUIContent[] CreateContents<T>(ToolbarChoice<T>[] options, bool showToggles)
		{
			EnsureStyles();

			return options.Select(
				c =>
				{
					if (!showToggles)
					{
						return c.Display;
					}
					else
					{
						return new GUIContent()
						{
							text = $" {c.Display.text}",
							tooltip = c.Display.tooltip,
							image = ToggleStateToTexture(c.ToggleState),
						};
					}

				}).ToArray();
		}

		private static Texture ToggleStateToTexture(ToolbarToggleState toggleState)
			=> toggleState switch
			{
				ToolbarToggleState.Normal  => _toggleNormal.image,
				ToolbarToggleState.Toggled => _toggleOn.image,
				ToolbarToggleState.Mixed   => _toggleMixed.image,
				_                          => throw new ArgumentOutOfRangeException(),
			};

		public static (int Result, bool WasToggle) DrawWithToggles<T>(Rect position, int current, ToolbarChoice<T>[] options)
		{
			if (options == null || options.Length == 0)
			{
				return (current, false);
			}

			EnsureStyles();

			int resultIndex = current;
			float horizontalSpacer = Constants.StandardHorizontalSpacing;
			Rect tabRect = position.EditorGUILineHeightTabs(options.Length, horizontalSpacer);

			for (int scan = 0; scan < options.Length; ++scan)
			{
				ToolbarChoice<T> opt = options[scan];

				float togglePx = RipoffParticleUI.Style.ToggleSquarePx;
				float spacer = RipoffParticleUI.Style.TogglePaddingPx * 2f;
				float buttonWidth = tabRect.width - togglePx - spacer;

				Rect toggleRect = tabRect.CollapseToLeft(togglePx).Height(togglePx);
				Rect selectRect = tabRect.CollapseToRight(buttonWidth);


				if (Event.current.type == EventType.Repaint)
				{
					GUI.DrawTexture(toggleRect, ToggleStateToTexture(opt.ToggleState));
					_buttonStyle.Draw(selectRect, opt.Display, false, false, scan == resultIndex, false);
				}
				else
				{	if (GUI.Button(toggleRect, GUIContent.none, GUIStyle.none))
					{
						opt.ToggleState = opt.ToggleState == ToolbarToggleState.Toggled
											  ? ToolbarToggleState.Normal
											  : ToolbarToggleState.Toggled;
						return (scan, true);
					}
					if (GUI.Button(selectRect, opt.Display, GUIStyle.none))
					{
						resultIndex = scan;
					}
				}
				tabRect.EditorGUINextTab(horizontalSpacer);
			}

			return (resultIndex, false);
		}

		public static int Draw<T>(Rect position, int current, ToolbarChoice<T>[] options)
		{
			int resultIndex = GUI.Toolbar(position, current, CreateContents(options, false));
			return resultIndex;
		}

		public static ToolbarChoice<T> Draw<T>(Rect position, string current, ToolbarChoice<T>[] options)
		{
			int currentIndex = string.IsNullOrEmpty(current) ? 0 : options.IndexOf(o => o.Text == current);
			int resultIndex = GUI.Toolbar(position, currentIndex, CreateContents(options, false));
			return options[resultIndex];
		}

		public static ToolbarChoice<T> Draw<T>(Rect position, ToolbarChoice<T> current, ToolbarChoice<T>[] options)
		{
			int currentIndex = Equals(current, default(T)) ? 0 : options.IndexOf(current);
			int resultIndex = GUI.Toolbar(position, currentIndex, CreateContents(options, false));
			return options[resultIndex];
		}

		public static ToolbarToggleState ToToggleState(this bool opt)
		=> opt
			   ? ToolbarToggleState.Toggled
			   : ToolbarToggleState.Normal;

		public static ToolbarToggleState ToToggleState(params bool[] opts)
		{
			if (opts == null || opts.Length == 0)
			{
				return ToolbarToggleState.Normal;
			}

			bool first = opts[0];

			if (opts.Length > 1)
			{
				for (int scan = 1; scan < opts.Length; ++scan)
				{
					if (opts[scan] != first)
					{
						return ToolbarToggleState.Mixed;
					}
				}
			}

			return first.ToToggleState();
		}
	}
}
