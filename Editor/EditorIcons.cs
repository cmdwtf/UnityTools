using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UnityEngine;


namespace cmdwtf.UnityTools.Editor
{
	/// <summary>
	/// An editor window to browse built-in editor icons,
	/// and optionally assign them to MonoScripts.
	/// </summary>
	public class EditorIcons : EditorWindow
	{
		/// <summary>
		/// Creates the editor window.
		/// </summary>
		[MenuItem(WindowMenuItem, priority = -1001)]
		public static void EditorIconsOpen()
		{
#if UNITY_2019_1_OR_NEWER
			EditorIcons window = CreateWindow<EditorIcons>(WindowTitle);
#else
			EditorIcons window = GetWindow<EditorIcons>(WindowTitle);
#endif // UNITY_2019_1_OR_NEWER

			window.minSize = new Vector2(WindowMinimumWidthPx, WindowMinimumHeightPx);
			window.ShowUtility();
		}

		// constants
		private const string WindowTitle = "Editor Icons";
		private const string WindowMenuItem = "Tools/Editor Icons %e";
		private const string RefreshIconName = "Refresh";
		private const float WindowNarrowWidthPx = 640f;
		private const float WindowMinimumWidthPx = 480f;
		private const float WindowMinimumHeightPx = 320f;
		private const float ButtonPaddingPx = 8f;
		private const float ButtonSizeSmallPx = 32f + ButtonPaddingPx;
		private const float ButtonSizeLargePx = 64f + ButtonPaddingPx;
		private const float ButtonSizeLargeDoublePx = ButtonSizeLargePx * 2f;
		private const float LargeSpacePx = 10f;
		private const float SmallSpacePx = LargeSpacePx / 2f;
		private const float DarkGrayValue = 0.15f;
		private const float LightGrayValue = 0.85f;
		private static readonly string[] DarkPreviewOptions = new[] { "Light", "Dark" };
		private static readonly string IconContentMethodFormat =
			$"{nameof(EditorGUIUtility)}.{nameof(EditorGUIUtility.IconContent)}(\"{{0}}\")";
		private static readonly string ObjectWrapperJsonHeader =
			@$"{nameof(UnityEditor)}.ObjectWrapperJSON:";
		private static readonly string ObjectWrapperJsonFormatObject =
			@"{{""guid"":""{0}"",""localId"":{1},""type"":0,""instanceID"":{2}}}";

		// style constants
		private static class Styles
		{
			public static readonly GUIStyle PreToolbar = "preToolbar";
			public static readonly GUIStyle PreToolbar2 = "preToolbar2";
			public static readonly GUIStyle PreToolbarLabel = "ToolbarBoldLabel";
			public static readonly GUIStyle PreDropDown = "preDropDown";
			public static readonly GUIContent CloseIcon = EditorGUIUtility.TrIconContent("winbtn_win_close");
			public static readonly GUIContent MenuIcon = EditorGUIUtility.TrIconContent("_Menu");
			public static readonly GUIContent DetailsHeader = new("Details");
		}

		// the icons themselves,
		// static to prevent unnecessary reloading
		private static EditorIconCache _cache;

		// user state
		private IconSize _showIconSize = IconSize.Large;
		private bool _showDarkPreview = true;
		private string _searchQuery = "";
		private EditorIconData _selectedIcon;
		private int _selectedIconNameIndex;
		private Object _selectedScript;
		private string SelectedIconName => _selectedIcon?.Names[_selectedIconNameIndex] ?? "<unknown>";
		private bool HasSelectedIcon => _selectedIcon != null;

		// ui components
		private SearchField _searchField;
		private GUIStyle _iconButtonStyle;
		private GUIStyle _iconPreviewDark;
		private GUIStyle _iconPreviewLight;
		private GUIContent _refreshIcon;

		// unity seems to clean up textures
		// that are only referenced through guistyle,
		// so for now keep explicit references to these.
		// Case: IN-25901
		private static Texture2D _darkGray;
		private static Texture2D _lightGray;

		// window state
		private Vector2 _scroll;

		private void OnEnable()
		{
			_cache ??= new EditorIconCache();
			InitializeGUIStyles();
		}

		private void OnGUI()
		{
			OnGUIToolbar();
			OnGUIIconGrid();
			OnGUIInfoPane();
		}

		/// <summary>
		/// Draws the window toolbar, including icon size choice and search field.
		/// </summary>
		private void OnGUIToolbar()
		{
			using GUILayout.HorizontalScope horizontalScope = new(EditorStyles.toolbar);

			GUILayout.Label("Icon Size:");
			_showIconSize = SelectionGrid(_showIconSize, EditorStyles.toolbarButton);

			_searchField ??= new SearchField();
			_searchQuery = _searchField.OnGUI(_searchQuery);

			if (GUILayout.Button(_refreshIcon, EditorStyles.toolbarButton))
			{
				_selectedIcon = null;
				_selectedIconNameIndex = 0;
				_cache = new EditorIconCache();
			}
		}

		/// <summary>
		/// Draws the icon grid, filtered by either the user's search query,
		/// or by the selected icon size.
		/// </summary>
		private void OnGUIIconGrid()
		{
			using GUILayout.ScrollViewScope scrollViewScope = new(_scroll);

			GUILayout.Space(LargeSpacePx);

			_scroll = scrollViewScope.scrollPosition;

			float buttonSize = _showIconSize == IconSize.Large
								   ? ButtonSizeLargePx
								   : ButtonSizeSmallPx;

			float scrollbarWidth = GUI.skin.verticalScrollbar.fixedWidth;
			float scaledScreenWidth = Screen.width / EditorGUIUtility.pixelsPerPoint;
			float renderWidth = scaledScreenWidth - scrollbarWidth;
			int iconsPerRow = Mathf.FloorToInt(renderWidth / buttonSize);
			float marginLeft = (renderWidth - (buttonSize * iconsPerRow)) / 2f;

			List<EditorIconData> iconList = _cache.Filter(_showIconSize, _searchQuery).ToList();
			int iconCount = iconList.Count();

			int iconsInRow = 0;

			for (int iconScan = 0; iconScan < iconCount; iconScan += iconsInRow)
			{
				using var horizontalScope = new GUILayout.HorizontalScope();
				GUILayout.Space(marginLeft);

				iconsInRow = Mathf.Min(iconCount - iconScan, iconsPerRow);

				for (int rowScan = 0; rowScan < iconsInRow; ++rowScan)
				{
					EditorIconData icon = iconList[iconScan + rowScan];
					GUIContent content = icon.Content;

					bool selected = GUILayout.Button(content,
													 _iconButtonStyle,
													 GUILayout.Width(buttonSize),
													 GUILayout.Height(buttonSize));

					if (!selected)
					{
						continue;
					}

					EditorGUI.FocusTextInControl(string.Empty);
					_selectedIcon = icon;
					_selectedIconNameIndex = 0;
				}
			}

			GUILayout.Space(LargeSpacePx);
		}

		/// <summary>
		/// Draws the info pane at the bottom of the window, if an icon is selected.
		/// </summary>
		private void OnGUIInfoPane()
		{
			if (!HasSelectedIcon)
			{
				return;
			}

			GUILayout.FlexibleSpace();

			EditorGUILayout.BeginHorizontal(Styles.PreToolbar, GUILayout.Height(EditorStyles.toolbar.fixedHeight));
			GUILayout.Label(Styles.DetailsHeader, Styles.PreToolbarLabel, GUILayout.ExpandWidth(false));
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(Styles.CloseIcon, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
			{
				_selectedIcon = null;
				_selectedIconNameIndex = 0;
			}

			EditorGUILayout.EndHorizontal();

			using GUILayout.HorizontalScope horizontalScope = new(EditorStyles.helpBox, GUILayout.MaxHeight(ButtonSizeLargeDoublePx), GUILayout.ExpandWidth(false));

			OnGUIInfoPanePreview();

			GUILayout.Space(LargeSpacePx);

			OnGUIInfoPaneDetails();

			GUILayout.Space(LargeSpacePx);
		}

		/// <summary>
		/// Draws the preview of the selected icon in the info pane.
		/// </summary>
		private void OnGUIInfoPanePreview()
		{
			if (!HasSelectedIcon)
			{
				return;
			}

			using  GUILayout.VerticalScope verticalScope = new(GUILayout.Width(ButtonSizeLargeDoublePx));

			GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

			// this button will server as our preview
			GUILayout.Button(_selectedIcon.Content,
							 _showDarkPreview ? _iconPreviewDark : _iconPreviewLight,
							 GUILayout.ExpandWidth(true),
							 GUILayout.MaxWidth(ButtonSizeLargeDoublePx),
							 GUILayout.Height(ButtonSizeLargeDoublePx));

			GUILayout.Space(SmallSpacePx);

			// allow the user to toggle between dark/light preview.
			_showDarkPreview = SelectionGrid(_showDarkPreview, DarkPreviewOptions, EditorStyles.miniButton);

			GUILayout.FlexibleSpace();
		}

		/// <summary>
		/// Draws the details of the selected icon in the info pane, as well as the actions
		/// to copy the icon to clipboard, or to assign it to a script.
		/// </summary>
		private void OnGUIInfoPaneDetails()
		{
			if (!HasSelectedIcon)
			{
				return;
			}

			using GUILayout.VerticalScope verticalScope = new(GUILayout.ExpandWidth(true));

			OnGUISelectedIconInformation();
			GUILayout.Space(SmallSpacePx);

			OnGUIAssignIconToScript();
			GUILayout.Space(SmallSpacePx);

			OnGUICopyIconAsUnityObject();
			GUILayout.Space(SmallSpacePx);

			OnGUICopyIconAsCode();
			GUILayout.Space(SmallSpacePx);

			OnGUISaveIconToFile();
		}

		/// <summary>
		/// Draws details about the selected icon.
		/// </summary>
		private void OnGUISelectedIconInformation()
		{
			if (!HasSelectedIcon)
			{
				return;
			}

			if (_selectedIcon.Count > 1)
			{
				using var horizontalScope = new GUILayout.HorizontalScope();

				string[] names = _selectedIcon.Names.ToArray();

				GUILayout.Label("Alternate Names: ", GUILayout.ExpandWidth(false));
				_selectedIconNameIndex = EditorGUILayout.Popup(_selectedIconNameIndex, names);
			}

			StringBuilder builder = new();

			builder.AppendLine($"Name: {SelectedIconName}");
			builder.AppendLine($"Size: {_selectedIcon.Dimensions}");
			builder.AppendLine($"Is Pro Skin: {_selectedIcon.ProSkinState}");
			builder.Append($"Total Icons: {_cache.TotalIconCount} ({_cache.UniqueIconCount} unique textures)");
			EditorGUILayout.HelpBox(builder.ToString(), MessageType.None);
		}

		/// <summary>
		/// Draws the interface that allows the user to copy the icon as either an <see>
		///     <cref>EditorGUIUtility.IconContent</cref>
		/// </see>
		/// call, or as just the icon name.
		/// </summary>
		private void OnGUICopyIconAsCode()
		{
			if (!HasSelectedIcon)
			{
				return;
			}

			string iconContentCall = string.Format(IconContentMethodFormat, SelectedIconName);
			EditorGUILayout.TextField(iconContentCall);

			using var horizontalScope = new GUILayout.HorizontalScope();

			GUILayout.Label("Copy:");

			if (GUILayout.Button("IconContent Call", EditorStyles.miniButton))
			{
				EditorGUIUtility.systemCopyBuffer = iconContentCall;
			}

			if (GUILayout.Button("Icon Name", EditorStyles.miniButton))
			{
				EditorGUIUtility.systemCopyBuffer = SelectedIconName;
			}
		}

		/// <summary>
		/// Draws the interface that allows the user to copy the details of the selected icon as Unity Object JSON,
		/// so they can paste it into an icon field.
		/// </summary>
		private void OnGUICopyIconAsUnityObject()
		{
			if (!HasSelectedIcon)
			{
				return;
			}

			// try and get the guid and file id of the icon. if we do,
			// we can copy it to the clipboard as an object.
			if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_selectedIcon.Image, out string guid,
															    out long localId))
			{
				return;
			}

			int instanceID = _selectedIcon.Image.GetInstanceID();

			if (GUILayout.Button("Copy Unity Object JSON", EditorStyles.miniButton))
			{
				EditorGUIUtility.systemCopyBuffer =
					ObjectWrapperJsonHeader +
					string.Format(ObjectWrapperJsonFormatObject, guid, localId, instanceID);
			}
		}

		/// <summary>
		/// Draws the interface that allows the user to select a script to assign the selected icon to.
		/// </summary>
		private void OnGUIAssignIconToScript()
		{
			if (!HasSelectedIcon)
			{
				return;
			}

			using var horizontalScope = new GUILayout.HorizontalScope();

			_selectedScript = EditorGUILayout.ObjectField(_selectedScript, typeof(MonoScript), false);
			string selectedScriptPath = AssetDatabase.GetAssetPath(_selectedScript);
			var scriptImporter = AssetImporter.GetAtPath(selectedScriptPath) as MonoImporter;

			using var disabledScope = new EditorGUI.DisabledScope(_selectedScript == null || scriptImporter == null);

			if (!GUILayout.Button("Assign to Script") || scriptImporter == null)
			{
				return;
			}

			scriptImporter.SetIcon(_selectedIcon.Image);
			scriptImporter.SaveAndReimport();
		}

		/// <summary>
		/// Draws the interface that allows the user to save the icon to a file on their system.
		/// </summary>
		private void OnGUISaveIconToFile()
		{
			if (!HasSelectedIcon)
			{
				return;
			}

			using var horizontalScope = new GUILayout.HorizontalScope();

			if (GUILayout.Button("Save to file...", EditorStyles.miniButton))
			{
				_selectedIcon.SaveToFile(_selectedIconNameIndex);
			}
		}
		/// <summary>
		/// Initializes the styles used by this editor window.
		/// </summary>
		private void InitializeGUIStyles()
		{
			_iconButtonStyle = new GUIStyle(EditorStyles.miniButton)
			{
				margin = new RectOffset(0, 0, 0, 0), fixedHeight = 0,
			};

			_darkGray ??= (new Color(DarkGrayValue, DarkGrayValue, DarkGrayValue)).ToTexture2DPixel();
			_lightGray ??= (new Color(LightGrayValue, LightGrayValue, LightGrayValue)).ToTexture2DPixel();

			_iconPreviewDark = new GUIStyle(_iconButtonStyle);
			_iconPreviewDark.name += "-dark";
			ApplyTextureToAllStyles(ref _iconPreviewDark, _darkGray);

			_iconPreviewLight = new GUIStyle(_iconButtonStyle);
			_iconPreviewLight.name += "-light";
			ApplyTextureToAllStyles(ref _iconPreviewLight, _lightGray);

			_refreshIcon = EditorGUIUtility.IconContent(RefreshIconName) ?? new GUIContent("Reload");
			_refreshIcon.tooltip = "Reload all icons";
		}

		/// <summary>
		/// A helper for a selection grid that deals with a boolean state.
		/// </summary>
		/// <param name="state">The current selection state.</param>
		/// <param name="choiceTexts">The texts to display for the selection. This array should have a length of 2.</param>
		/// <param name="style">The style to draw the selection grid with.</param>
		/// <returns>The modified selection state.</returns>
		private static bool SelectionGrid(bool state, string[] choiceTexts, GUIStyle style)
		{
			int selected = state ? 1 : 0;
			selected = GUILayout.SelectionGrid(selected, choiceTexts, choiceTexts.Length,
											   style ?? EditorStyles.miniButton);

			state = selected != 0;
			return state;
		}

		/// <summary>
		/// A helper for a selection grid that deals with an enumerable state.
		/// </summary>
		/// <param name="state">The current selection state.</param>
		/// <param name="style">The style to draw the selection grid with.</param>
		/// <typeparam name="TEnum">The type of enum the selection grid is based on.</typeparam>
		/// <returns>The modified selection state.</returns>
		private static TEnum SelectionGrid<TEnum>(TEnum state, GUIStyle style) where TEnum : System.Enum
		{
			if (System.Enum.GetValues(typeof(TEnum)) is not TEnum[] enumOptions)
			{
				Debug.LogWarning($"Unable to get values for type: {typeof(TEnum).Name}");
				return state;
			}

			int selected = enumOptions.IndexOf(state);

			selected = GUILayout.SelectionGrid(selected,
											   enumOptions.Select(e  => $"{e}").ToArray(),
											   enumOptions.Length,
											   style ?? EditorStyles.miniButton);

			return enumOptions[selected];
		}

		/// <summary>
		/// A helper that assigns the given texture to all the background and stretched background
		/// styles of the given <see cref="GUIStyle"/>.
		/// </summary>
		/// <param name="style">The style to modify.</param>
		/// <param name="texture">The texture to assign to the style.</param>
		private static void ApplyTextureToAllStyles(ref GUIStyle style, Texture2D texture)
		{
			// assign all backgrounds
			style.hover.background =
				style.onHover.background =
					style.focused.background =
						style.onFocused.background =
							style.active.background =
								style.onActive.background =
									style.normal.background =
										style.onNormal.background = texture;

			// assign all scaled backgrounds
			style.hover.scaledBackgrounds =
				style.onHover.scaledBackgrounds =
					style.focused.scaledBackgrounds =
						style.onFocused.scaledBackgrounds =
							style.active.scaledBackgrounds =
								style.onActive.scaledBackgrounds =
									style.normal.scaledBackgrounds =
										style.onNormal.scaledBackgrounds = new[] { texture };
		}
	}
}
