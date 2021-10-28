using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor
{
	internal static class AutohookEditorSettingsProvider
    {
		[SettingsProvider]
		public static SettingsProvider CreateAutohookSettingsProvider()
		{
			var provider = new SettingsProvider("Project/Autohook Settings", SettingsScope.Project)
			{
				guiHandler = (searchContext) =>
				{
					var settings = AutohookSettings.GetOrCreateSettings();
					var serialized = settings.GetSerialized();
					EditorGUILayout.PropertyField(serialized.FindProperty(nameof(AutohookSettings.defaultVisibility)), new GUIContent("Default Visibility"));

					settings.Validate();
				},

				// Populate the search keywords to enable smart search filtering and label highlighting:
				keywords = new HashSet<string>(new[] { "Autohook", "Visibility" }),
			};

			return provider;
		}
    }
}
