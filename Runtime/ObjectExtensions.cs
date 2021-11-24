using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class ObjectExtensions
	{
		public static string GetAssetName(this Object obj)
		{
#if !UNITY_EDITOR
			throw new System.NotSupportedException($"{nameof(GetAssetName)} is only supported in the editor.");
#else // UNITY_EDITOR
			string assetPath = AssetDatabase.GetAssetPath(obj.GetInstanceID());

			return !string.IsNullOrEmpty(assetPath)
					   ? Path.GetFileNameWithoutExtension(assetPath)
					   : null;
#endif // !UNITY_EDITOR
		}

		public static bool TryGetAssetName(this Object obj, out string assetName)
		{
			try
			{
				assetName = obj.GetAssetName();
				return assetName != null;
			}
			catch (System.Exception /*e*/)
			{
				assetName = null;
			}

			return false;
		}
	}
}
