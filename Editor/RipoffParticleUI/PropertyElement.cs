using System;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor.RipoffParticleUI
{
	public class PropertyElement : Element, ISimplePropertyContainer
	{
		private bool _isExpandedFallback = true;
		public override bool IsExpanded
		{
			get => AssociatedProperty?.isExpanded ?? _isExpandedFallback;
			set
			{
				if (AssociatedProperty != null)
				{
					AssociatedProperty.isExpanded = value;
				}

				_isExpandedFallback = value;
			}
		}

		public string AssociatedPropertyName { get; private set; }

		public SerializedProperty AssociatedProperty { get; private set; }

		// disable the default constructor, we need a property name!
		private PropertyElement() { }

		public PropertyElement(string propertyName, GUIContent contentOverride = null)
		{
			AssociatedPropertyName = propertyName;
			Content = contentOverride;
		}

		private void ValidateAssociatedProperty(Context context)
		{
			if (AssociatedProperty == null)
			{
				if (AssociatedPropertyName == null)
				{
					// the user didn't specify a property, associate with the
					// whole object instead.
					AssociatedProperty = context.RootProperty;
				}

				AssociatedProperty ??=
					context.CurrentSerializedProperty?.FindPropertyRelative(AssociatedPropertyName) ??
					context.SerializedObject.FindProperty(AssociatedPropertyName);

				if (AssociatedProperty == null)
				{
					throw new MissingMemberException(
						$"Unable to find the associated property {AssociatedPropertyName} via the serialized object or current context property.");
				}

				// must call Next(true) to get to the first property returned.
				//AssociatedProperty.Next(true);
			}

			context.CurrentSerializedProperty = AssociatedProperty;
		}

		#region Overrides of Element

		/// <inheritdoc />
		protected override void OnBeforeDraw(Context context)
		{
			base.OnBeforeDraw(context);
			ValidateAssociatedProperty(context);
			Content ??= new GUIContent(AssociatedProperty.displayName, AssociatedProperty.tooltip);
		}

		/// <inheritdoc />
		protected override void OnAfterDraw(Context context)
		{
			base.OnAfterDraw(context);
			AssociatedProperty.Reset();
			AssociatedProperty = null;
		}

		#endregion

		public SimplePropertyContainer SimpleProperties { get; private set; } = new();

		#region ISimplePropertyContainer Delegation

		/// <inheritdoc />
		public SimpleProperty AddSimpleProperty(string propertyName, GUIContent label = null) => SimpleProperties.AddSimpleProperty(propertyName, label);

		/// <inheritdoc />
		public SimpleProperty AddSimpleProperty(string propertyName, string label) => SimpleProperties.AddSimpleProperty(propertyName, label);

		/// <inheritdoc />
		public void RemoveSimpleProperty(string propertyName) => SimpleProperties.RemoveSimpleProperty(propertyName);

		/// <inheritdoc />
		public void SetSimplePropertyVisible(string propertyName, bool isVisible) => SimpleProperties.SetSimplePropertyVisible(propertyName, isVisible);

		#region Implementation of IPropertyContainer

		/// <inheritdoc />
		public virtual void OnDrawProperties(Context context) => SimpleProperties.OnDrawProperties(context);

		#endregion

		#endregion
	}
}
