using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

using UnityEngine;

namespace cmdwtf.UnityTools
{
	/// <summary>
	/// A base class for a singleton-style <see cref="ScriptableObject"/> that is available at runtime. For editor-only
	/// functionality, see Unity's <see cref="ScriptableSingleton{T}"/> instead.
	/// </summary>
	/// <typeparam name="T">The scriptable object type to create in a single instance style.</typeparam>
	public class UniqueScriptableObject<T> : ScriptableObject where T : ScriptableObject
	{
		private const string UniqueNamespaceDefault = "Unique";

		private static T _instance = null;

		/// <summary>
		/// The single instance of the scriptable object.
		/// </summary>
		public static T Instance => _instance ??= CreateAndLoad();

		/// <summary>
		/// The "Resources" relative path that the unique object will be loaded from.
		/// </summary>
		protected static string AssetResourcePath
		{
			get
			{
				string assetName = $"{typeof(T).Name}.asset";
				string ns = GetUniqueNamespace();

				return string.IsNullOrEmpty(ns)
						   ? assetName
						   : $"{ns}/{assetName}";
			}
		}

		protected virtual void Reset() { }
		protected virtual void OnEnable() { }
		protected virtual void Awake() { }
		protected virtual void OnDisable() { }
		protected virtual void OnDestroy() => _instance = null;
		protected virtual void OnValidate() { }

		private static T CreateAndLoad()
		{
			string resourceDirectory = GetUniqueNamespace();
			T[] loaded = Resources.LoadAll<T>(resourceDirectory);

			if (loaded == null)
			{
#if UNITY_EDITOR
				// in the editor, if we don't find our unique asset, we will create it.
				loaded = new[] { CreateOrSave(null) };
#endif // UNITY_EDITOR

				if (loaded == null && Application.isPlaying)
				{
					Exception missingReason = new($"Found no resource of type {typeof(T).Name}");
					throw new TypeInitializationException(typeof(T).FullName, missingReason);
				}
			}

			if (loaded.Length == 1)
			{
				_instance = loaded[0];
				return _instance;
			}

			Exception multipleReason = new($"Found {loaded.Length} {typeof(T).Name}s, expected 1.");
			throw new TypeInitializationException(typeof(T).FullName, multipleReason);
		}

		private static string GetUniqueNamespace()
			=> typeof(T)
			   .GetCustomAttributes(true)
			   .OfType<UniqueNamespaceAttribute>()
			   .FirstOrDefault()?.Namespace ??
			   UniqueNamespaceDefault;

#if UNITY_EDITOR

		private const string ResourcesFilePath = "Assets/Resources";

		/// <summary>
		/// Saves the instance to it's asset file.
		/// </summary>
		public virtual void Save() => CreateOrSave(_instance);

		private static T CreateOrSave(T toSave)
		{
			//bool isCreating = AssetDatabase.Contains(this) == false;
			bool isCreating = toSave == null;
			string path = ResourceAssetFilePath;

			if (isCreating)
			{
				toSave = CreateInstance<T>();
				System.IO.Directory.CreateDirectory(path);
				AssetDatabase.CreateAsset(toSave, path);
				EditorUtility.SetDirty(toSave);
			}

			AssetDatabase.SaveAssetIfDirty(toSave);

			if (!isCreating)
			{
				return toSave;
			}

			EditorGUIUtility.PingObject(toSave);

			return toSave;
		}

		/// <summary>
		/// The project-relative file path used to save the asset in the editor.
		/// </summary>
		protected static string ResourceAssetFilePath => $"{ResourcesFilePath}/{AssetResourcePath}";

#endif // UNITY_EDITOR
	}
}
