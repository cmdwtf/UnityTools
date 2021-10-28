using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	internal class AutohookSettings : ScriptableObject
	{
		private const string AutohookSettingsPath = "Assets/Editor/AutohookSettings.asset";

		[SerializeField]
		internal Visibility defaultVisibility;

		internal static AutohookSettings GetOrCreateSettings()
		{
			var settings = AssetDatabase.LoadAssetAtPath<AutohookSettings>(AutohookSettingsPath);

			if (settings != null)
			{
				return settings;
			}

			settings = CreateInstance<AutohookSettings>();
			settings.defaultVisibility = Visibility.Visible;
			AssetDatabase.CreateAsset(settings, AutohookSettingsPath);
			AssetDatabase.SaveAssets();

			return settings;
		}

		internal static SerializedObject GetSerializedSettings()
			=> new SerializedObject(GetOrCreateSettings());
	}
}
