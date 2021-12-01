using System;
using System.Collections.Generic;

namespace cmdwtf.UnityTools
{
	public static class CollectionExtensions
	{
		public static int IndexOf<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
		{
			// using foreach to prevent full enumeration,
			// which might be required to count.
			int scan = 0;
			foreach (T item in collection)
			{
				if (predicate(item))
				{
					return scan;
				}

				++scan;
			}
			return -1;
		}
	}
}
