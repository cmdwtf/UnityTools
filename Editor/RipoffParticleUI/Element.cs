using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor.RipoffParticleUI
{
	public class Element : IElement
	{
		public virtual bool IsExpanded { get; set; } = true;

		public virtual bool Visible { get; set; } = true;

		public List<Element> Children { get; private set; } = new();

		public virtual bool ChildrenSupported { get; protected set; } = true;

		public event Func<Element, bool> UpdateVisibility;

		public GUIContent Content { get; protected set; }

		public Element()
			: this(GUIContent.none)
		{

		}

		public Element(GUIContent content)
		{
			Content = content;
		}

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
