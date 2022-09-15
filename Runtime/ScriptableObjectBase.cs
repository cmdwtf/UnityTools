using System;

using UnityEngine;

namespace cmdwtf.UnityTools
{
	public abstract class ScriptableObjectBase : ScriptableObject
	{
		public string Name => displayName;
		public string displayName;

		public event Action Validated;

		protected virtual void OnValidate()
		{
#if UNITY_EDITOR
			displayName ??= this.GetAssetName() ?? string.Empty;
#endif // UNITY_EDITOR

			if (string.IsNullOrEmpty(name))
			{
				name = displayName;
			}

			Validated?.Invoke();
		}

		protected virtual void Reset()
		{
#if UNITY_EDITOR
			displayName = null;
			OnValidate();
#endif // UNITY_EDITOR
		}
	}
}
