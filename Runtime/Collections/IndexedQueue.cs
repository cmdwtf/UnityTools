using System;
using System.Collections.Generic;

namespace cmdwtf.UnityTools.Collections
{
	/// <summary>
	/// Represents a queue with indexed access to the items
	/// </summary>
	/// <typeparam name="T">The type of items in the queue</typeparam>
	public partial class IndexedQueue<T>
	{
		const int _DefaultCapacity = 16;
		const float _GrowthFactor = 2f;
		const float _TrimThreshold = .9f;
		T[] _array;
		int _head;
		int _count;
		/// <summary>
		/// Creates a queue with the specified capacity
		/// </summary>
		/// <param name="capacity">The initial capacity of the queue</param>
		public IndexedQueue(int capacity) {
			if (0 >= capacity)
				throw new ArgumentOutOfRangeException("The capacity must be greater than zero.", nameof(capacity));
			_array = new T[capacity];
			_head = 0;
			_count = 0;
			_version = 0;
		}
		/// <summary>
		/// Gets or sets the value at the index
		/// </summary>
		/// <param name="index">The index</param>
		/// <returns>The value at the specified index</returns>
		public T this[int index] {
			get {
				if (0 > index || index >= _count)
					throw new IndexOutOfRangeException();
				return _array[(index + _head) % _array.Length];
			}
			set {
				if (0 > index || index >= _count)
					throw new IndexOutOfRangeException();
				_array[(index + _head) % _array.Length] = value;
				++_version;
			}
		}
		/// <summary>
		/// Creates a queue with the default capacity
		/// </summary>
		public IndexedQueue() : this(_DefaultCapacity)
		{

		}
		/// <summary>
		/// Creates a queue filled with the specified items
		/// </summary>
		/// <param name="collection">The collection of items to fill the queue with</param>
		public IndexedQueue(IEnumerable<T> collection)
		{
			if (null == collection)
				throw new ArgumentNullException(nameof(collection));
			foreach (var item in collection)
				Enqueue(item);
		}
		/// <summary>
		/// Provides access to the item at the front of the queue without dequeueing it
		/// </summary>
		/// <returns>The frontmost item</returns>
		public T Peek()
		{
			if (0 == _count)
				throw new InvalidOperationException("The queue is empty.");
			return _array[_head];
		}
		/// <summary>
		/// Returns an array of the items in the queue
		/// </summary>
		/// <returns>An array containing the queue's items</returns>
		public T[] ToArray()
		{
			var result = new T[_count];
			CopyTo(result, 0);
			return result;
		}
		/// <summary>
		/// Inserts an item at the rear of the queue
		/// </summary>
		/// <param name="item">The item to insert</param>
		public void Enqueue(T item)
		{
			if(_count==_array.Length)
			{
				var arr = new T[(int)(_array.Length * _GrowthFactor)];
				if(_head+_count<=_array.Length)
				{
					Array.Copy(_array, arr, _count);
					_head = 0;
					arr[_count] = item;
					++_count;
					unchecked
					{
						++_version;
					}
					_array = arr;
				} else // if(_head+_count<=arr.Length)
				{
					Array.Copy(_array, _head, arr, 0, _array.Length - _head);
					Array.Copy(_array, 0, arr, _array.Length - _head, _head);
					_head = 0;
					arr[_count] = item;
					++_count;
					unchecked
					{
						++_version;
					}
					_array = arr;
				}
			} else
			{
				_array[(_head + _count) % _array.Length] = item;
				++_count;
				unchecked
				{
					++_version;
				}
			}
		}
		/// <summary>
		/// Removes an item from the front of the queue, returning it
		/// </summary>
		/// <returns>The item that was removed</returns>
		public T Dequeue()
		{
			if (0 == _count)
				throw new InvalidOperationException("The queue is empty");
			var result = _array[_head];
			++_head;
			_head = _head % _array.Length;
			--_count;
			unchecked
			{
				++_version;
			}
			return result;
		}
		/// <summary>
		/// Trims the extra array space that isn't being used.
		/// </summary>
		public void TrimExcess()
		{
			if (0 == _count)
			{
				_array = new T[_DefaultCapacity];
			}
			if(_array.Length*_TrimThreshold>=_count)
			{
				var arr = new T[_count];
				CopyTo(arr, 0);
				_head = 0;
				_array = arr;
				unchecked
				{
					++_version;
				}
			}
		}
		/// <summary>
		/// Attempts to return an item from the front of the queue without removing it
		/// </summary>
		/// <param name="item">The item</param>
		/// <returns>True if the queue returned an item or false if the queue is empty</returns>
		public bool TryPeek(out T item)
		{
			if(0!=_count)
			{
				item = _array[_head];
				return true;
			}
			item = default(T);
			return false;
		}
		/// <summary>
		/// Attempts to return an item from the front of the queue, removing it
		/// </summary>
		/// <param name="item">The item</param>
		/// <returns>True if the queue returned an item or false if the queue is empty</returns>
		public bool TryDequeue(out T item)
		{
			if (0 < _count)
			{
				item = _array[_head];
				++_head;
				_head = _head % _array.Length;
				--_count;
				unchecked
				{
					++_version;
				}
				return true;
			}
			item = default(T);
			return false;
		}
	}
}
