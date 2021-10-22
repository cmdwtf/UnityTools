using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.PlayerLoop;

namespace cmdwtf.UnityTools.Editor
{
	public abstract class CustomEditorBase : UnityEditor.Editor
	{
		private List<IEnumerator> _runningCoroutines = new List<IEnumerator>();
		private string ObjectName => target == null ? string.Empty : target.name;

		protected CustomEditorBase()
		{
			EditorApplication.update += Update;
		}

		private void Update()
		{
			if (_runningCoroutines.Count <= 0)
			{
				return;
			}

			int index = _runningCoroutines.Count - 1;
			
			Debug.Log($"{ObjectName}: Executing Coroutine {index}");
         
			bool coroutineFinished = !_runningCoroutines[index].MoveNext();
 
			if (coroutineFinished)
			{
				Debug.Log($"{ObjectName}: Finished Coroutine {index}");
				_runningCoroutines.RemoveAt(index);
			}
		}
		
		protected void DoChildEditor(Component child, bool recurse = true)
		{
			if (child == null)
			{
				return;
			}

			var serialized = new SerializedObject(child);
			SerializedProperty prop = serialized.GetIterator();
			prop.NextVisible(true);

			while (prop.NextVisible(recurse))
			{
				if (prop.name != "m_Script") // && prop.name.StartsWith("m_"))
				{
					EditorGUILayout.PropertyField(prop);
				}
			}

			prop.Reset();

			serialized.ApplyModifiedProperties();
		}
		
		protected void StartCoroutine(IEnumerator cr)
		{
			_runningCoroutines.Add(cr);
		}
	}
}