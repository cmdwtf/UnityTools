using System;
using System.Globalization;

using UnityEngine;

using Random = UnityEngine.Random;

namespace cmdwtf.UnityTools
{
	[Serializable]
	public struct ValueRange : IEquatable<ValueRange>, IFormattable
	{
		[SerializeField]
		internal Vector2 range;
		[SerializeField]
		internal Vector2 limit;
		[SerializeField]
		public bool canEditLimit;

		public float minimum
		{
			get => range.x;
			set => range.x = value < limit.x ? limit.x : value;
		}

		public int minimumInt
		{
			get => Mathf.RoundToInt(range.x);
			set => range.x = value < limit.x ? limit.x : value;
		}

		public float maximum
		{
			get => range.y;
			set => range.y = value > limit.y ? limit.y : value;
		}

		public int maximumInt
		{
			get => Mathf.RoundToInt(range.y);
			set => range.y = value > limit.y ? limit.y : value;
		}

		public float minimumLimit
		{
			get => limit.x;
			set => limit.x = canEditLimit ? value : limit.x;
		}

		public float maximumLimit
		{
			get => limit.y;
			set => limit.y = canEditLimit ? value : limit.y;
		}
		
		// static instances of common value ranges
		private static ValueRange zeroOrEmpty = new ValueRange(0.0f, 0.0f);
		public static ValueRange empty { get; } = zeroOrEmpty;
		public static ValueRange zero { get; } = zeroOrEmpty;
		public static ValueRange zeroOne { get; } = new ValueRange(0.0f, 1.0f);
		public static ValueRange one { get; } = new ValueRange(1.0f, 1.0f);
		
		public ValueRange(float minimumValue, float maximumValue, float? minimumLimit = null, float? maximumLimit = null)
		{
			limit = new Vector2(
				minimumLimit ?? minimumValue,
				maximumLimit ?? maximumValue
			);
			
			range = new Vector2(
				minimumValue,
				maximumValue
			);

			canEditLimit = false;
		}

		public float Random() => UnityEngine.Random.Range(minimum, maximum);

		public int RandomInt() => UnityEngine.Random.Range(minimumInt, maximumInt);

		public float Sample(float t) => Mathf.Lerp(minimum, maximum, t);

		public float SampleUnclamped(float t) => Mathf.LerpUnclamped(minimum, maximum, t);

		public override string ToString()
			=> $"(Min: {minimum.ToString2Points()}, Max: {maximum.ToString2Points()})";

		public string ToString(string format)
			=> ToString(format, CultureInfo.InvariantCulture.NumberFormat);
		
		public string ToString(string format, IFormatProvider formatProvider)
			=> range.ToString(format, formatProvider);

		public string ToFullString()
			=> $"[{minimumLimit.ToString2Points()}] " +
			   $"{minimum.ToString2Points()} - " +
			   $"{maximum.ToString2Points()} " +
			   $"[{maximumLimit.ToString2Points()}]";

		public bool Equals(ValueRange other) => range.Equals(other.range) && limit.Equals(other.limit);
	}
}