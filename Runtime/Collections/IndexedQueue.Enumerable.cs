
using System;
using System.Collections;
using System.Collections.Generic;

namespace cmdwtf.UnityTools.Collections
{
	partial class IndexedQueue<T> : IEnumerable<T>
	{
		int _version;
		public IEnumerator<T> GetEnumerator()
			=>new Enumerator(this);
		// legacy enumerable support (required)
		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
		struct Enumerator : IEnumerator<T>
		{
			const int _AfterEnd = -1;
			const int _BeforeStart = -2;
			const int _Disposed = -3;

			IndexedQueue<T> _outer;
			int _index;
			int _version;
			public Enumerator(IndexedQueue<T> outer)
			{
				_outer = outer;
				_version = outer._version;
				_index = _BeforeStart;
			}
			public T Current {
				get {
					switch (_index)
					{
						case _Disposed:
							throw new ObjectDisposedException(GetType().Name);
						case _BeforeStart:
							throw new InvalidOperationException("The cursor is before the start of the enumeration.");
						case _AfterEnd:
							throw new InvalidOperationException("The cursor is after the end of the enumeration.");
					}
					_CheckVersion();
					return _outer._array[_index % _outer._array.Length];
				}
			}
			// legacy enumerator support (required)
			object IEnumerator.Current
				=> Current;
			public bool MoveNext()
			{
				switch(_index)
				{
					case _Disposed:
						throw new ObjectDisposedException(GetType().Name);
					case _AfterEnd:
						return false;
					case _BeforeStart:
						_CheckVersion();
						if(0==_outer._count)
						{
							_index = _AfterEnd;
							return false;
						}
						_index = _outer._head;
						return true;
				}
				_CheckVersion();
				if (++_index >= _outer._count+_outer._head)
				{
					_index = _AfterEnd;
					return false;
				}
				return true;
			}
			public void Reset()
			{
				if (-3 == _index)
					throw new ObjectDisposedException(GetType().Name);
				_CheckVersion();
				_index = _BeforeStart;
			}
			public void Dispose()
			{
				_index = _Disposed;
			}
			void _CheckVersion()
			{
				if (_version != _outer._version)
					throw new InvalidOperationException("The enumeration has changed and may not execute.");
			}
		}
	}
}
