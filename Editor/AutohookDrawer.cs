using System;
using System.Reflection;

using cmdwtf.UnityTools.Attributes;

using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace cmdwtf.UnityTools.Editor
{
    [UnityEditor.CustomPropertyDrawer(typeof(AutohookAttribute), useForChildren: true)]
    internal sealed class AutohookDrawer : CustomPropertyDrawer
    {
        private AutohookVisibility _autohookVisibility;

        private AutohookAttribute Attribute => (AutohookAttribute)attribute;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Component component = InjectNow(property);

            if (component != null && _autohookVisibility == AutohookVisibility.Hidden)
            {
				return;
			}

            bool guiEnabled = GUI.enabled;

            if (component != null && _autohookVisibility == AutohookVisibility.Disabled)
			{
				GUI.enabled = false;
			}

			EditorGUI.PropertyField(position, property, label);
            GUI.enabled = guiEnabled;
        }

        private void UpdateVisibility()
		{
            _autohookVisibility = Attribute.Visibility;

            if (_autohookVisibility == AutohookVisibility.Default)
			{
				var settings = AutohookSettings.GetOrCreateSettings();
				_autohookVisibility = settings.defaultVisibility;
			}
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            UpdateVisibility();

            Component component = InjectNow(property);
            if (component != null && _autohookVisibility == AutohookVisibility.Hidden)
			{
				return 0;
			}

			return base.GetPropertyHeight(property, label);
        }

		private Component InjectNow(SerializedProperty property)
			=> Attribute.Temporality != AutohookTemporality.Editor
				   ? null
				   : AutohookInjection.InjectSerializedReference(Attribute, property);
	}
}
