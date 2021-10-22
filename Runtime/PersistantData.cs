using System;
using System.IO;

using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class PersistantData
	{
		public static StreamingAssets.SettingsCollection Settings => StreamingAssets.Settings;
		private static string Tag => StreamingAssets.Settings.LogTag;
		
		public static DateTime GetFileModifiedTime(string path) => File.GetLastWriteTime(path);

		/// <summary>
		/// Deletes all files matching the search pattern from the persistent data path.
		/// </summary>
		/// <param name="searchPattern">The pattern to match</param>
		/// <param name="subdir">A subdirectory (if any) to search in</param>
		/// <param name="option">Search option to recurse through directories or search top level only</param>
		public static void RemoveFiles(string searchPattern, string subdir = null, SearchOption option = SearchOption.TopDirectoryOnly)
		{
			string directory = Path.Combine(Application.persistentDataPath, (subdir ?? ""));
			string[] files = Directory.GetFiles(directory, searchPattern, option);

			foreach (string file in files)
			{
				try
				{
					File.Delete(file);
				}
				catch (Exception ex)
				{
					Debug.LogError($"{Tag}Error deleting {file}: {ex.Message}");
				}
			}
		}
	}
}
