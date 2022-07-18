using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor.RipoffParticleUI
{
	/// <summary>
	/// A property element that represents a single, simple property.
	/// </summary>
	public sealed class SimpleProperty : PropertyElement
	{
		/// <inheritdoc />
		public override bool IsExpanded => true;

		/// <summary>
		/// The potential modes to draw the simple property in.
		/// </summary>
		public enum PropertyMode
		{
			Single,
			Full,
			ChildrenOnly,
		}

		/// <summary>
		/// The mode to draw the simple property in.
		/// </summary>
		public PropertyMode Mode { get; set; } = PropertyMode.Single;

		private readonly string _propertyName;

		public SimpleProperty(string propertyName, GUIContent content = null)
			: base(propertyName, content)
		{

		}

		internal void Draw(Context context)
		{
			OnBeforeDraw(context);
			OnDraw(context);
			OnAfterDraw(context);
		}

		/// <inheritdoc />
		protected override void OnDraw(Context context)
		{
			if (AssociatedProperty == null)
			{
				Debug.LogWarning($"Unable to find property: {_propertyName}");
				return;
			}

			Content ??= new GUIContent(AssociatedProperty.displayName, AssociatedProperty.tooltip);

			bool showChildren = Mode != PropertyMode.Single;

			if (Mode == PropertyMode.ChildrenOnly && AssociatedProperty.hasChildren)
			{
				AssociatedProperty.Next(true);

				var end = AssociatedProperty.GetEndProperty();

				while (AssociatedProperty.Next(true))
				{
					EditorGUILayout.PropertyField(AssociatedProperty, Content, showChildren);
				}

				return;
			}

			EditorGUILayout.PropertyField(AssociatedProperty, Content, showChildren);
		}
	}
}
