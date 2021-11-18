using System;
using System.Collections.Generic;

namespace cmdwtf.UnityTools.Collections
{
	partial class IndexedQueue<T> : ICollection<T>
	{
		/// <summary>
		/// Indicates the count of items in the queue
		/// </summary>
		public int Count => _count;
		bool ICollection<T>.IsReadOnly => false;

		void ICollection<T>.Add(T item)
			=>Enqueue(item);

		/// <summary>
		/// Clears the items from the queue
		/// </summary>
		public void Clear()
		{
			_head = 0;
			_count = 0;
			unchecked
			{
				++_version;
			}
		}
		/// <summary>
		/// Indicates whether the queue contains an item
		/// </summary>
		/// <param name="item">The item to search for</param>
		/// <returns>True if the item was found, otherwise false</returns>
		public bool Contains(T item)
		{
			// TODO: Reimplement this using IndexOf()
			for(var i = 0;i<_count;++i)
				if (Equals(_array[(_head + i) % _array.Length], item))
					return true;
			return false;
		}
		/// <summary>
		/// Copys the queue's items to a destination array
		/// </summary>
		/// <param name="array">The destination array</param>
		/// <param name="arrayIndex">The index in the destination to start copying at</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			if (null == array) throw new ArgumentNullException(nameof(array));
			if (0 > arrayIndex || array.Length < arrayIndex + _count)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));
			// TODO: Reimplement this using Array.Copy
			for (var i = 0; i < _count; ++i)
				array[arrayIndex + i] = _array[(_head + i) % _array.Length];
		}

		bool ICollection<T>.Remove(T item)
		{
			if (Equals(_array[_head],item))
			{
				Dequeue();
				return true;
			}
			else
			{
				// TODO: Reimplement using RemoveAt()/IndexOf()
				for(var i = 0;i<_count;++i)
				{
					var idx = (_head + i) % _array.Length;
					if(Equals(_array[idx],item))
					{
						if (_head + _count < _array.Length)
						{
							Array.Copy(_array, idx + 1, _array, idx, _count - idx - 1);
						}
						else if (idx == _array.Length - 1)
						{
							_array[idx] = _array[0];
							if(_count+_head!=_array.Length)
							{
								Array.Copy(_array, 1, _array, 0, (_count + _head) % _array.Length - 1);
							}
						}
						else if (idx < _head)
						{
							Array.Copy(_array, idx + 1, _array, idx, (_count + _head) % _array.Length - 1);
						}
						--_count;
						unchecked
						{
							++_version;
						}
						return true;
					}
				}
			}
			return false;
		}
	}
}
