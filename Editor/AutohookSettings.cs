using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace cmdwtf.UnityTools.Editor
{
	internal class AutohookSettings : ScriptableObject
	{
		private const string AutohookSettingsPath = "Assets/Editor/AutohookSettings.asset";

		public const Visibility DefaultDefaultVisibility = Visibility.Visible;

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
			settings.Validate();
			AssetDatabase.CreateAsset(settings, AutohookSettingsPath);
			AssetDatabase.SaveAssets();

			return settings;
		}

		internal SerializedObject GetSerialized() => new SerializedObject(this);

		internal static SerializedObject GetOrCreateSerialized()
			=> new SerializedObject(GetOrCreateSettings());

		public void Validate()
		{
			if (defaultVisibility == Visibility.Default)
			{
				defaultVisibility = DefaultDefaultVisibility;
			}
		}
	}
}
