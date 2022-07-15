using System;
using System.Collections.Generic;
using System.IO;

#if USE_DOTNETZIP
using Ionic.Zip;
#endif // USE_DOTNETZIP

using UnityEngine;
using UnityEngine.Networking;

namespace cmdwtf.UnityTools
{
	public static partial class StreamingAssets
	{
		public static SettingsCollection Settings { get; } = new SettingsCollection();

		private static string Tag => Settings.LogTag;

		public static DateTime GetFileModifiedTime(string path)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				try
				{
#if USE_DOTNETZIP
					using (var apk = ZipFile.Read(Application.dataPath))
					{
						foreach (ZipEntry entry in apk.Entries)
						{
							// ignore directories
							if (entry.IsDirectory)
							{
								continue;
							}

							if (entry.FileName == path)
							{
								// LastModified, not ModifiedTime.
								// See: http://www.nudoq.org/#!/Packages/DotNetZip/Ionic.Zip/ZipEntry/P/ModifiedTime
								return entry.LastModified;
							}
						}
					}
#endif // USE_DOTNETZIP
					Debug.LogError(
						$"{Tag}Entry not found when trying to retrieving last modified time from zip for path: {path}");
				}
				catch (Exception ex)
				{
					Debug.LogError($"{Tag}Error retrieving last modified time from zip for path: {path}, {ex.Message}");
				}

				return DateTime.MinValue;
			}

			return File.GetLastWriteTime(path);
		}

		/// <summary>
		/// Checks if the given file exists in the persistent data path. If it does,
		/// just returns that path. Otherwise, copies the file from the streaming assets.
		/// </summary>
		/// <returns>The persistant file path of the given file, or null on www error or file not existing in streaming.</returns>
		/// <param name="filename">The file name to look for/load from streaming.</param>
		/// <param name="subDirectory">An optional subDirectory to search in.</param>
		public static string CopyToPersistent(string filename, string subDirectory = null)
		{
			// get the full dest directory
			string directory = Path.Combine(Application.persistentDataPath, (subDirectory ?? ""));
			// ensure the directory exists
			if (Directory.Exists(directory) == false)
			{
				Directory.CreateDirectory(directory);
			}

			// check if file exists in Application.persistentDataPath
			string destinationFilepath = Path.Combine(directory, filename);

			bool overwriting = false;
			bool overwritingBecauseNewer = false;

			// if we have a setting to overwrite, delete it if it exists
			if (Settings.OverwriteOnCopyFromStreaming)
			{
				if (File.Exists(destinationFilepath))
				{
					try
					{
						File.Delete(destinationFilepath);
						overwriting = true;
					}
					catch (Exception ex)
					{
						Debug.LogWarning(
							$"{Tag}(Overwrite) Unable to remove file from persistent: {destinationFilepath}, {ex.Message}");
					}
				}
			}
			else if (Settings.OverwriteOnCopyFromStreamingIfNewer)
			{
				string assetPath = BuildStreamingAssetsPath(filename, subDirectory, true);

				if (File.Exists(destinationFilepath)
					&& GetFileModifiedTime(assetPath) > PersistantData.GetFileModifiedTime(destinationFilepath))
				{
					try
					{
						File.Delete(destinationFilepath);
						overwriting = true;
						overwritingBecauseNewer = true;

						Debug.Log(
							$"{Tag}(CopyIfNewer) Overwriting persistent {destinationFilepath} with streaming asset: {assetPath} because the modified time of the streaming asset is more recent than the persistent path!");
					}
					catch (Exception ex)
					{
						Debug.LogWarning(
							$"{Tag}(CopyIfNewer) Unable to remove file from persistent: {destinationFilepath}, {ex.Message}");
					}
				}
			}

			if (!File.Exists(destinationFilepath))
			{
				// if it doesn't ->
				// open StreamingAssets directory and load the file ->
				// copy it to the persistant data path.

				if (Application.platform == RuntimePlatform.Android)
				{
					string assetPath = "jar:file://";

					string streamingassetspath = $"{Application.dataPath}!/assets/";

					if (subDirectory != null)
					{
						streamingassetspath = Path.Combine(streamingassetspath, subDirectory);
						streamingassetspath += "/";
					}

					assetPath = assetPath + streamingassetspath + filename;

					var uwr = UnityWebRequest.Get(assetPath);
					uwr.SendWebRequest();

					if (!overwriting)
					{
						Debug.Log(
							$"{Tag}(Android) File \"{destinationFilepath}\" does not exist. Attempting to create from: {assetPath}");
					}
					else if (overwritingBecauseNewer)
					{
						Debug.Log(
							$"{Tag}(Android) File \"{destinationFilepath}\" is newer in streaming assets. Attempting to create from: {assetPath}");
					}

					while (!uwr.isDone) { }

					if (string.IsNullOrEmpty(uwr.error) == false)
					{
						Debug.LogWarning($"{Tag}Error copying file: {uwr.error}");
						return null;
					}

					// then save to Application.persistentDataPath
					File.WriteAllBytes(destinationFilepath, uwr.downloadHandler.data);

					if (File.Exists(destinationFilepath))
					{
						if (overwriting == false)
						{
							Debug.Log($"{Tag}(Android) File \"{destinationFilepath}\" created.");
						}
					}
					else
					{
						Debug.LogError($"{Tag}(Android) File \"{destinationFilepath}\" NOT created.");
					}
				}
				else
				{
					string assetPath = Path.Combine(Path.Combine(Application.streamingAssetsPath, subDirectory ?? ""), filename);

					if (File.Exists(assetPath) == false)
					{
						Debug.LogError($"{Tag}{filename} not found in StreamingAssets. Unable to copy file.");
						return null;
					}

					if (!overwriting)
					{
						Debug.Log(
							$"{Tag}(Default Platform) File \"{destinationFilepath}\" does not exist. Attempting to create from: {assetPath}");
					}
					else if (overwritingBecauseNewer)
					{
						Debug.Log(
							$"{Tag}(Default Platform) File \"{destinationFilepath}\" is newer in streaming assets. Attempting to create from: {assetPath}");
					}

					File.Copy(assetPath, destinationFilepath);

					if (File.Exists(destinationFilepath))
					{
						if (overwriting == false)
						{
							Debug.Log($"{Tag}(Default Platform) File \"{destinationFilepath}\" created.");
						}
					}
					else
					{
						Debug.LogError($"{Tag}(Default Platform) File \"{destinationFilepath}\" NOT created.");
					}

				}

			}

			return destinationFilepath;
		}

		public static void CopyAllToPersistant(string searchPattern, string subDirectory = null)
		{
			try
			{
#if UNITY_ANDROID
				// fix subDirectory slashes for android.
				if (subDirectory != null && Application.platform == RuntimePlatform.Android)
				{
					subDirectory = subDirectory.Replace('\\', '/');
				}
#endif // UNITY_ANDROID

				string directory = Path.Combine(Application.streamingAssetsPath, (subDirectory ?? ""));
				string[] files;

				// look for streaming assets to copy
				if (Application.platform == RuntimePlatform.Android)
				{
					files = GetAllAndroid(searchPattern, subDirectory);
				}
				else
				{
					files = GetAllWindows(searchPattern, directory);
				}

				for (int scan = 0; scan < files.Length; ++scan)
				{
					files[scan] = files[scan].Replace(directory, string.Empty).TrimStart(new char[] { '\\', '/' });
				}

				foreach (string file in files)
				{
					CopyToPersistent(file, subDirectory);
				}
			}
			catch (Exception ex)
			{
				Debug.Log($"Exception trying to copy all streaming assets to persistent ({searchPattern}): {ex.Message}");
			}
		}

		private static string[] GetAllWindows(string searchPattern, string directory = null)
			=> Directory.GetFiles(directory ?? string.Empty, searchPattern, SearchOption.TopDirectoryOnly);

		private static string[] GetAllAndroid(string searchPattern, string subDirectory = null)
		{
			subDirectory ??= "";

			var found = new List<string>();

			var matcher = new Wildcard(searchPattern, System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
#if USE_DOTNETZIP
			using var apk = ZipFile.Read(Application.dataPath);

			foreach (ZipEntry entry in apk.Entries)
			{
				// ignore directories
				if (entry.IsDirectory)
				{
					continue;
				}

				string filenameOnly = Path.GetFileName(entry.FileName);
				string pathOnly = Path.GetDirectoryName(entry.FileName);

				// in the apk, all of our streaming assets are in /assets
				string target = "assets";

				// are we looking in a subdirectory?
				if (string.IsNullOrEmpty(subDirectory) == false)
				{
					target = $"{target}/{subDirectory}";
				}

				if (pathOnly != target)
				{
					continue;
				}

				// if it matches our pattern, add it to the return.
				if (matcher.IsMatch(filenameOnly))
				{
					found.Add(filenameOnly);
				}
			}

			return found.ToArray();
#else
			throw new NotSupportedException();
#endif // USE_DOTNETZIP
		}

		private static string BuildStreamingAssetsPath(string filename, string subDirectory, bool androidInZipPath)
		{
			string assetPath;

			if (Application.platform == RuntimePlatform.Android)
			{
				if (androidInZipPath)
				{
					// build the SA path as if we were looking into the zip.
					return $"assets/{(subDirectory == null ? "" : $"{subDirectory}/")}{filename}";
				}
				else
				{
					// build the SA path for android directly from the jar.
					assetPath = Settings.AndroidJarPrefix;

					string streamingAssetsPath = $"{Application.dataPath}!/assets/";

					if (subDirectory != null)
					{
						streamingAssetsPath = Path.Combine(streamingAssetsPath, subDirectory);
						streamingAssetsPath += "/";
					}

					assetPath = assetPath + streamingAssetsPath + filename;
				}
			}
			else
			{
				// build the SA path for Non-android
				assetPath = Path.Combine(Application.streamingAssetsPath, (subDirectory ?? ""));
				assetPath = Path.Combine(assetPath,                       filename);
			}

			return assetPath;
		}
	}
}
