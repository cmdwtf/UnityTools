using cmdwtf.UnityTools.UI;

using UnityEditor;
using UnityEditor.UI;

namespace cmdwtf.UnityTools.Editor
{
	/// <summary>
	/// An editor for <see cref="HeldButton"/> to allow them to be multi-edited.
	/// </summary>
	[CustomEditor(typeof(HeldButton), true)]
	[CanEditMultipleObjects]
	public class HeldButtonEditor : ButtonEditor
	{
		private SerializedProperty _onPressedProperty;
		private SerializedProperty _onReleasedProperty;

		#region Overrides of ButtonEditor

		/// <inheritdoc />
		protected override void OnEnable()
		{
			base.OnEnable();
			_onPressedProperty = serializedObject.FindProperty(nameof(HeldButton._onPress));
			_onReleasedProperty = serializedObject.FindProperty(nameof(HeldButton._onRelease));
		}

		/// <inheritdoc />
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			serializedObject.Update();
			EditorGUILayout.PropertyField(_onPressedProperty);
			EditorGUILayout.PropertyField(_onReleasedProperty);
			serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}
