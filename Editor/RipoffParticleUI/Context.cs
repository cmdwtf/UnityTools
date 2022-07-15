using System;
using System.Collections.Generic;

using UnityEditor;

namespace cmdwtf.UnityTools.Editor.RipoffParticleUI
{
	public class Context
	{
		private SerializedObject _serializedObject;

		public SerializedObject SerializedObject
		{
			get => _serializedObject;
			set
			{
				_serializedObject = value;
				CurrentSerializedProperty = null;
			}
		}

		public SerializedProperty RootProperty => SerializedObject.GetIterator();

		public SerializedProperty CurrentSerializedProperty { get; set; }

		public Style Styles { get; } = new Style();

		private Stack<SerializedProperty> _properties = new();
	}
}
