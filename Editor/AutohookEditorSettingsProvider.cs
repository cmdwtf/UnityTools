using System.Collections.Generic;

using cmdwtf.UnityTools.Attributes;

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

					EditorGUI.BeginChangeCheck();
					settings.defaultVisibility =
						(AutohookVisibility)EditorGUILayout.EnumPopup("Default Visibility", settings.defaultVisibility);

					if (!EditorGUI.EndChangeCheck())
					{
						return;
					}

					settings.Validate();
					settings.MarkDirty();
				},

				// Populate the search keywords to enable smart search filtering and label highlighting:
				keywords = new HashSet<string>(new[] { "Autohook", "Visibility" }),
			};

			return provider;
		}
    }
}
