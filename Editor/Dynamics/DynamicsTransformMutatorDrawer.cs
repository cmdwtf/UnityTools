using System.Collections.Generic;
using System.Linq;

using cmdwtf.UnityTools.Dynamics;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor.Dynamics
{
	[UnityEditor.CustomPropertyDrawer(typeof(DynamicsTransformMutator), useForChildren: true)]
	public class DynamicsTransformMutatorDrawer : CustomPropertyDrawer
	{
		private static readonly HashSet<string> SkipProperties = new() { nameof(DynamicsTransformMutator.enabled) };

		#region Overrides of CustomPropertyDrawer

		/// <inheritdoc />
		protected override int PropertyLineCount { get; set; } = 0;

		/// <inheritdoc />
		protected override int GetPropertyLineCount(SerializedProperty property, GUIContent label) => 0;

		/// <inheritdoc />
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;

		// rather than rendering the property directly, we'll do the kids!
		/// <inheritdoc />
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
			=> ChildrenPropertyFieldsLayout(property, SkipProperties);

		#endregion
	}
}
