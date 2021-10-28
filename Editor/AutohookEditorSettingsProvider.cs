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
					var settings = AutohookSettings.GetSerializedSettings();
					EditorGUILayout.PropertyField(settings.FindProperty(nameof(AutohookSettings.defaultVisibility)),     new GUIContent("Default Visibility"));
				},

				// Populate the search keywords to enable smart search filtering and label highlighting:
				keywords = new HashSet<string>(new[] { "Autohook", "Visibility" }),
			};

			return provider;
		}
    }
}
