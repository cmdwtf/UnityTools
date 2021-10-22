namespace cmdwtf.UnityTools
{
	public static partial class StreamingAssets
	{
		public class SettingsCollection
		{
			public bool OverwriteOnCopyFromStreaming { get; set; }
			public bool OverwriteOnCopyFromStreamingIfNewer { get; set; }
			public string AndroidJarPrefix { get; private set; }
			public string LogTag { get; set; }

			internal SettingsCollection()
			{
				Revert();
			}

			public void Revert()
			{
				OverwriteOnCopyFromStreaming = false;
				OverwriteOnCopyFromStreamingIfNewer = true;
				LogTag = "[CMD] ";
				AndroidJarPrefix = "jar:file://";
			}
		}
	}
}
