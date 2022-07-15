using System;
using UnityEngine;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace cmdwtf.UnityTools
{
#if UNITY_2020_1_OR_NEWER
	[Serializable]
	public struct Optional<T>
	{
		[SerializeField]
		internal bool enabled;
		[SerializeField]
		internal T value;

		public bool Enabled => enabled;
		public T Value => value;

		public Optional(T initialValue)
		{
			enabled = true;
			value = initialValue;
		}

		public static implicit operator Optional<T>(T initialValue) => new(initialValue);
		public static explicit operator T(Optional<T> value) => value.Enabled ? value.Value : default;

		public override string ToString() => Value.ToString();
	}
#endif // UNITY_2020_1_OR_NEWER
}
