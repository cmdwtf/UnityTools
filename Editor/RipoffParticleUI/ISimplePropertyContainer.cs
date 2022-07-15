using UnityEngine;

namespace cmdwtf.UnityTools.Editor.RipoffParticleUI
{
	public interface ISimplePropertyContainer : IPropertyContainer
	{
		SimpleProperty AddSimpleProperty(string propertyName, GUIContent label = null);
		SimpleProperty AddSimpleProperty(string propertyName, string label);
		void RemoveSimpleProperty(string propertyName);
		void SetSimplePropertyVisible(string propertyName, bool isVisible);
	}
}
