namespace cmdwtf.UnityTools
{
	public static class NetLegacy
	{
#if NET_LEGACY
		public static class Path
		{
			public static string Combine(params string [] paths)
			{
				if (paths == null || paths.Length == 0)
				{
					return string.Empty;
				}

				if (paths.Length == 1)
				{
					return paths[0];
				}

				string ret = paths[0];
				for (int pathsScan = 1; pathsScan < paths.Length; pathsScan++)
				{
					ret = Path.Combine(ret, paths[pathsScan]);
				}

				return ret;
			}
		}
#endif // NET_LEGACY
	}
}
