using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.PlayerLoop;

namespace cmdwtf.UnityTools.Editor
{
	public abstract class CustomEditorBase : UnityEditor.Editor
	{
		protected readonly List<string> IgnoredPropertyNames = new List<string>{ "m_Script" };

		private readonly List<IEnumerator> _runningCoroutines;
		private string ObjectName => target == null ? string.Empty : target.name;

		protected CustomEditorBase()
		{
			_runningCoroutines = new List<IEnumerator>();
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

		protected bool DoInspector(SerializedObject serialized, bool skipFirst = false, bool recurse = true, bool showHidden = false)
		{
			SerializedProperty prop = serialized.GetIterator();

			// next(true) must be called to get the first element.

			System.Func<bool, bool> nextFunc = showHidden
				? prop.Next
				: prop.NextVisible;

			nextFunc(true);

			if (skipFirst)
			{
				EditorGUILayout.PropertyField(prop);
			}

			while (nextFunc(recurse))
			{
				if (IgnoredPropertyNames.Contains(prop.name) == false || !showHidden)
				{
					EditorGUILayout.PropertyField(prop);
				}
			}

			prop.Reset();

			return serialized.ApplyModifiedProperties();
		}

		protected void DoChildInspector(Component child, bool recurse = true)
		{
			if (child == null)
			{
				return;
			}

			var serialized = new SerializedObject(child);

			DoInspector(serialized, skipFirst: true, recurse);
		}

		protected void StartCoroutine(IEnumerator cr) => _runningCoroutines.Add(cr);
	}
}
