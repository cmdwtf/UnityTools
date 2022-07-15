using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Editor.RipoffParticleUI
{
	public sealed class SimplePropertyContainer : ISimplePropertyContainer
	{
		private class PropertyMeta
		{
			public string Name => PropertyElement.AssociatedPropertyName;

			public bool Visible { get; set; } = true;

			public SimpleProperty PropertyElement { get; set; }

			public PropertyMeta(string name, GUIContent label)
			{
				PropertyElement = new SimpleProperty(name, label);
			}
		}

		public bool IncludeHiddenProperties { get; set; } = false;

		private readonly Dictionary<string, PropertyMeta> _simpleProperties = new();

		/// <inheritdoc />
		public SimpleProperty AddSimpleProperty(string propertyName, GUIContent label = null)
		{
			if (!_simpleProperties.ContainsKey(propertyName))
			{
				_simpleProperties.Add(propertyName, new PropertyMeta(propertyName, label));
			}

			return _simpleProperties[propertyName].PropertyElement;
		}

		/// <inheritdoc />
		public SimpleProperty AddSimpleProperty(string propertyName, string label)
			=> AddSimpleProperty(propertyName,
								 string.IsNullOrWhiteSpace(label) ? GUIContent.none : new GUIContent(label));

		/// <inheritdoc />
		public void RemoveSimpleProperty(string propertyName) => _simpleProperties.Remove(propertyName);

		/// <inheritdoc />
		public void SetSimplePropertyVisible(string propertyName, bool isVisible)
		{
			if (_simpleProperties.TryGetValue(propertyName, out PropertyMeta meta))
			{
				meta.Visible = isVisible;
			}
		}

		/// <summary>
		/// Draws calls the <see cref="SimpleProperty.OnDraw"/> method
		/// of all of the visible simple properties.
		/// </summary>
		/// <param name="context">The drawing context.</param>
		public void OnDrawProperties(Context context)
		{
			foreach (PropertyMeta meta in _simpleProperties.Values)
			{
				if (!meta.Visible && !IncludeHiddenProperties)
				{
					continue;
				}

				meta.PropertyElement.Draw(context);
			}
		}
	}
}
