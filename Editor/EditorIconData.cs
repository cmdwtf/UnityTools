using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	internal enum IconProSkinState
	{
		Free,
		Pro,
		Both
	}

	internal class EditorIconData
	{
		private const string ProIconPrefix = "d_";

		public string Name
		{
			get => _names.FirstOrDefault() ?? string.Empty;
			private set
			{
				if (_names.Any())
				{
					_names[0] = value;
					return;
				}

				_names.Add(value);
			}
		}
		public GUIContent Content { get; private set; }
		public string Path { get; private set; }

		public IReadOnlyList<string> Names => _names.AsReadOnly();

		public string AllNames => GetAllNames();

		public bool IsValid => Content != null;

		public IconProSkinState ProSkinState
		{
			get
			{
				bool hasPro = _names.Any(IsProName);

				if (Count == 1)
				{
					return hasPro
							   ? IconProSkinState.Pro
							   : IconProSkinState.Free;
				}

				return IconProSkinState.Both;
			}
		}

		public float Width => Content?.image.width ?? 0;
		public float Height => Content?.image.height ?? 0;
		public string Dimensions => $"{Width}x{Height}";

		public Texture2D Image => Content?.image as Texture2D;

		public string SortName => GetSortName(Name);

		/// <summary>
		/// The number of names this icon represents.
		/// </summary>
		public int Count => _names.Count;

		private readonly List<string> _names = new();

		public bool IsLarge => Size == IconSize.Large;
		public bool IsMedium => Size == IconSize.Medium;
		public bool IsSmall => Size == IconSize.Small;

		public IconSize Size
		{
			get
			{
				if (!IsValid)
				{
					throw new InvalidOperationException($"{nameof(Size)} should not be accessed on invalid icon data.");
				}

				if (Width < 32 && Height < 32)
				{
					return IconSize.Small;
				}

				if (Width < 48 && Height < 48)
				{
					return IconSize.Medium;
				}

				return IconSize.Large;
			}
		}

		public static EditorIconData Empty { get; } = new();

		public EditorIconData()
		{
			Name = string.Empty;
			Content = null;
			Path = string.Empty;
		}

		public static EditorIconData FromIconName(string iconName)
		{
			GUIContent content = GetIcon(iconName);

			if (content == null || content.image == null)
			{
				return Empty;
			}

			return new EditorIconData(iconName, content);
		}

		public EditorIconData(Texture2D t2d)
		{
			if (t2d == null)
			{
				throw new ArgumentNullException(nameof(t2d));
			}

			Name = t2d.name;
			Content = GetIcon(t2d.name);
			Path = AssetDatabase.GetAssetPath(t2d);

			if (Content != null)
			{
				Content.tooltip = Name;
			}
		}

		private EditorIconData(string iconName, GUIContent iconContent)
		{
			Name = iconName ?? throw new ArgumentNullException(nameof(iconName));
			Content = iconContent ?? throw new ArgumentNullException(nameof(iconContent));
			Path = AssetDatabase.GetAssetPath(iconContent.image);

			Content.tooltip = Name;
		}

		public bool MatchesName(string nameToTest)
		{
			string testSortName = GetSortName(nameToTest);
			return _names.Any(n => GetSortName(n) == testSortName);
		}

		public void AddAlternateName(string alternateName)
		{
			_names.Add(alternateName);
		}

		public void AddSecondaryTextureName(string name) => _names.Add(name);

		public bool MatchesSearchQuery(string query)
		{
			string lowerQuery = query.ToLowerInvariant();
			return string.IsNullOrWhiteSpace(query) ||
				   SortName.Contains(lowerQuery) ||
				   (_names?.Any(alt => alt.ToLowerInvariant().Contains(lowerQuery)) ?? false);
		}

		public string GetAllNames() => string.Join(", ", Names);

		public static bool IsProName(string name) => name.StartsWith(ProIconPrefix);

		public bool SaveToFile(int selectedNameIndex)
		{
			string name = _names[selectedNameIndex];
			var source = Content.image as Texture2D;

			if (source != null)
			{
				string path = EditorUtility.SaveFilePanel(
					"Save Icon", string.Empty, name, "png");

				if (path != null)
				{
					try
					{
#if UNITY_2019_1_OR_NEWER
                    var outTex = new Texture2D(
                        source.width, source.height,
						source.format, source.mipmapCount, true);
#else
					Texture2D outTex = new Texture2D(
						tex.width, tex.height,
						tex.format, true);
#endif

						Graphics.CopyTexture(source, outTex);

						System.IO.File.WriteAllBytes(path, outTex.EncodeToPNG());
						return true;
					}
					catch (System.Exception e)
					{
						Debug.LogError($"Failed to save icon: {e.Message}");
					}
				}
			}
			else
			{
				Debug.LogError($"Failed to save icon: no texture found.");
			}

			return false;
		}

		private static GUIContent GetIcon(string iconName)
		{
			GUIContent valid = null;
			Debug.unityLogger.logEnabled = false;
			if (!string.IsNullOrEmpty(iconName))
			{
				valid = EditorGUIUtility.IconContent(iconName);
			}

			Debug.unityLogger.logEnabled = true;
			return valid?.image == null ? null : valid;
		}

		private static string GetSortName(string name)
			=> (name.StartsWith(ProIconPrefix)
					? name.Substring(2)
					: name).ToLowerInvariant();

		// private static string Listify(string[] names, bool newLines)
		// {
		// 	if (!names.Any())
		// 	{
		// 		return "<empty set>";
		// 	}
		//
		// 	return "\"" + string.Join((newLines ? "\", \"\r\n" : "\", \""), names) + "\"";
		// }
		//
		// private static void WriteToFile(StreamWriter w, string header, IEnumerable<string> values, bool newlines = false)
		// {
		// 	string[] enumerable = values as string[] ?? values.ToArray();
		// 	int count = enumerable.Count();
		// 	string value = Listify(enumerable, newlines);
		// 	w.WriteLine($"=== {header} ({count}) ===");
		// 	w.WriteLine();
		// 	w.WriteLine(value);
		// 	w.WriteLine("---");
		// }
		//
		// public static void FileIt(string header, IEnumerable<string> things)
		// {
		// 	string path = System.IO.Path.GetTempFileName() + $".{header}.txt";
		// 	using StreamWriter file = new(path);
		// 	WriteToFile(file, header, things);
		// 	ProcessStartInfo psi = new()
		// 	{
		// 		Arguments = $"{path}", FileName = @"C:\Program Files\Notepad++\notepad++.exe",
		// 	};
		// 	Process.Start(psi);
		// }
	}
}
