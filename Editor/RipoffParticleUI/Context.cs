using System;
using System.Collections.Generic;

using UnityEditor;

namespace cmdwtf.UnityTools.Editor.RipoffParticleUI
{
	/// <summary>
	/// The drawing context passed through the steps of rendering a <see cref="RipoffParticleUI"/> tree.
	/// </summary>
	public class Context
	{
		private SerializedObject _serializedObject;

		/// <summary>
		/// The serialized object associated with this draw phase.
		/// </summary>
		public SerializedObject SerializedObject
		{
			get => _serializedObject;
			set
			{
				_serializedObject = value;
				CurrentSerializedProperty = null;
			}
		}

		/// <summary>
		/// A helper to quickly access the root property of the <see cref="SerializedObject"/>.
		/// </summary>
		public SerializedProperty RootProperty => SerializedObject.GetIterator();

		/// <summary>
		/// The property currently being referenced by a given <see cref="Element"/>. This value should
		/// be considered ephemeral, and only valid in the context of any given element.
		/// </summary>
		public SerializedProperty CurrentSerializedProperty { get; set; }

		/// <summary>
		/// UI Styles for the <see cref="RipoffParticleUI"/> to use.
		/// </summary>
		public Style Styles { get; } = new();
	}
}
