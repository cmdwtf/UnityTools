using System;
using System.Collections;
using System.Collections.Generic;

namespace cmdwtf.UnityTools.Collections
{
	internal static class CollectionHelpers
	{
		public static IReadOnlyCollection<T> ReifyCollection<T>(IEnumerable<T> source)
			=> source switch
			{
				null                             => throw new ArgumentNullException(nameof(source)),
				IReadOnlyCollection<T> result    => result,
				ICollection<T> collection        => new CollectionWrapper<T>(collection),
				ICollection nongenericCollection => new NongenericCollectionWrapper<T>(nongenericCollection),
				_                                => new List<T>(source),
			};

		private sealed class NongenericCollectionWrapper<T> : IReadOnlyCollection<T>
		{
			private readonly ICollection _collection;

			public NongenericCollectionWrapper(ICollection collection)
			{
				if (collection == null)
				{
					throw new ArgumentNullException(nameof(collection));
				}

				_collection = collection;
			}

			public int Count => _collection.Count;

			public IEnumerator<T> GetEnumerator()
			{
				foreach (T item in _collection)
				{
					yield return item;
				}
			}

			IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();
		}

		private sealed class CollectionWrapper<T> : IReadOnlyCollection<T>
		{
			private readonly ICollection<T> _collection;

			public CollectionWrapper(ICollection<T> collection)
			{
				_collection = collection ?? throw new ArgumentNullException(nameof(collection));
			}

			public int Count => _collection.Count;

			public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();
		}
	}
}
