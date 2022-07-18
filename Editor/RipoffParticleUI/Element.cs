using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor.RipoffParticleUI
{
	/// <summary>
	/// A UI Element. This class should be the root type of any type drawn in the <see cref="RipoffParticleUI"/> tree.
	/// </summary>
	public abstract class Element : IElement
	{
		/// <summary>
		/// <see langword="true" />, if the element should be expanded showing its children.
		/// </summary>
		public virtual bool IsExpanded { get; set; } = true;

		/// <summary>
		/// <see langword="true" />, if the element should be drawn at all.
		/// </summary>
		public virtual bool Visible { get; set; } = true;

		/// <summary>
		/// Elements that belong to this element node.
		/// </summary>
		public List<Element> Children { get; private set; } = new();

		/// <summary>
		/// <see langword="true" />, if the element supports owning child element nodes.
		/// </summary>
		public virtual bool ChildrenSupported { get; protected set; } = true;

		/// <summary>
		/// Raised before drawing the element to allow user code to update if the element should be visible.
		/// </summary>
		public event Func<Element, bool> UpdateVisibility;

		/// <summary>
		/// Content drawn with the element.
		/// </summary>
		public GUIContent Content { get; protected set; }

		public Element()
			: this(GUIContent.none)
		{

		}

		public Element(GUIContent content)
		{
			Content = content;
		}

		/// <summary>
		/// Adds the given <see cref="Element"/> as a child of this element.
		/// </summary>
		/// <param name="newChild">The child to add.</param>
		/// <returns><see langword="true" /> if the element was added, <see langword="false" /> if it already belonged to this element.</returns>
		public virtual bool Add(Element newChild)
		{
			if (Children.Contains(newChild))
			{
				return false;
			}

			Children.Add(newChild);
			return true;
		}

		protected virtual void OnBeforeDraw(Context context) => Visible = UpdateVisibility?.Invoke(this) ?? Visible;

		protected virtual void OnDraw(Context context)
		{
			void DrawChildren(Element node)
			{
				foreach (Element child in node.Children)
				{
					child.OnBeforeDraw(context);

					if (child.Visible == false)
					{
						continue;
					}

					child.OnDraw(context);

					if (child.IsExpanded)
					{
						if (child is IPropertyContainer pc)
						{
							pc.OnDrawProperties(context);
						}

						using EditorGUI.IndentLevelScope indent = new();
						DrawChildren(child);
					}

					child.OnAfterDraw(context);
				}
			}

			DrawChildren(this);
		}

		protected virtual void OnAfterDraw(Context context) { }
	}
}
