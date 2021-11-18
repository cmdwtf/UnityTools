using System.IO;

using cmdwtf.UnityTools.Attributes;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace cmdwtf.UnityTools.Editor
{
	internal class AutohookSettings : ScriptableObject
	{
		private const string AutohookSettingsPath = "Assets/Editor/AutohookSettings.asset";

		public const AutohookVisibility DefaultDefaultVisibility = AutohookVisibility.Visible;

		[SerializeField]
		internal AutohookVisibility defaultVisibility;

		internal static AutohookSettings GetOrCreateSettings()
		{
			string settingsDir = Path.GetDirectoryName(AutohookSettingsPath);
			if (Directory.Exists(settingsDir) == false)
			{
				Debug.Log($"Creating {nameof(AutohookSettings)} directory: {settingsDir}");
				Directory.CreateDirectory(AutohookSettingsPath);
			}

			var settings = AssetDatabase.LoadAssetAtPath<AutohookSettings>(AutohookSettingsPath);

			if (settings != null)
			{
				return settings;
			}

			settings = CreateInstance<AutohookSettings>();

			// create the asset
			AssetDatabase.CreateAsset(settings, AutohookSettingsPath);
			AssetDatabase.SaveAssets();

			return settings;
		}

		internal SerializedObject GetSerialized() => new SerializedObject(this);

		internal static SerializedObject GetOrCreateSerialized()
			=> new SerializedObject(GetOrCreateSettings());

		public void Validate()
		{
			if (defaultVisibility == AutohookVisibility.Default)
			{
				defaultVisibility = DefaultDefaultVisibility;
			}
		}

		public void MarkDirty() => EditorUtility.SetDirty(this);
	}
}
