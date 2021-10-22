namespace cmdwtf.UnityTools
{
	public static class ArrayExtensions
	{
		public static T[] SetAllValues<T>(this T[] array, T value) where T : struct
		{
			for (int scan = 0; scan < array.Length; scan++)
			{
				array[scan] = value;
			}

			return array;
		}
	}
}
