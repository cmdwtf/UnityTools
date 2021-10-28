using UnityEditor;

namespace BrunoMikoski.Framework.AutoHook
{
    public static class AutoHookEditorSettings
    {
        public enum VisibilityInSettings
        {
            Visible = 1,
            Disabled = 2,
            Hidden = 4
        }

        public const string AUTO_HOOK_VISIBILITY_KEY = "AUTO_HOOK_defaultVisibility";

        private static int? defaultVisibilityIndex;

        private static VisibilityInSettings selectedVisibility;

        [PreferenceItem("Autohook Settings")]
        public static void PreferencesGUI()
        {
            if (!defaultVisibilityIndex.HasValue)
            {
                defaultVisibilityIndex = EditorPrefs.GetInt(AUTO_HOOK_VISIBILITY_KEY, 0);

                selectedVisibility = (VisibilityInSettings)defaultVisibilityIndex;
            }

            EditorGUI.BeginChangeCheck();
            selectedVisibility =
                (VisibilityInSettings)EditorGUILayout.EnumPopup("Visibility", selectedVisibility);

            if (EditorGUI.EndChangeCheck())
            {
                defaultVisibilityIndex = (int)selectedVisibility;
                EditorPrefs.SetInt(AUTO_HOOK_VISIBILITY_KEY, defaultVisibilityIndex.Value);

                for (int i = 0; i < Selection.objects.Length; i++)
                    EditorUtility.SetDirty(Selection.objects[i]);
            }
        }
    }
}
