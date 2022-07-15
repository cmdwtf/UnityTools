using System;
using System.Globalization;

using UnityEngine;

// ReSharper disable InconsistentNaming

namespace cmdwtf.UnityTools
{
	/// <summary>
	/// A simple class representing a floating point value range that has limits on it's minimum and maximum.
	/// This can be used with the associated property drawer to generate a min-max slider in the Editor.
	/// </summary>
	[Serializable]
	public struct ValueRange : IEquatable<ValueRange>, IFormattable, ISerializationCallbackReceiver
	{
		private delegate void AssignmentAction(ref ValueRange arg1);

		#region Serialized Fields

		[SerializeField]
		private Vector2 range;

		[SerializeField]
		private Vector2 limit;

		#endregion

		private bool _isValidating;

		/// <summary>
		/// The minimum value of the range.
		/// </summary>
		public float minimum
		{
			get => range.x;
			set => InvokeAndValidate((ref ValueRange t) => t.range.x = value < t.limit.x ? t.limit.x : value);
		}

		/// <summary>
		/// The minimum value of the range, as an <see langword="int"/>.
		/// </summary>
		public int minimumInt
		{
			get => Mathf.RoundToInt(range.x);
			set => minimum = value;
		}

		/// <summary>
		/// The maximum value of the range.
		/// </summary>
		public float maximum
		{
			get => range.y;
			set => InvokeAndValidate((ref ValueRange t) => t.range.y = value > t.limit.y ? t.limit.y : value);
		}

		/// <summary>
		/// The maximum value of the range, as an <see langword="int"/>.
		/// </summary>
		public int maximumInt
		{
			get => Mathf.RoundToInt(range.y);
			set => maximum = value;
		}

		/// <summary>
		/// The value of the range, as a tuple.
		/// </summary>
		public (float min, float max) value
		{
			get => (range.x, range.y);
			set => InvokeAndValidate((ref ValueRange t) => t.range = new Vector2(value.min, value.max));
		}

		/// <summary>
		/// The minimum limit that the minimum value of the range may be.
		/// </summary>
		public float minimumLimit
		{
			get => limit.x;
			set => InvokeAndValidate((ref ValueRange t) => t.limit.x = value);
		}

		/// <summary>
		/// The maximum limit that the maximum value of the range may be.
		/// </summary>
		public float maximumLimit
		{
			get => limit.y;
			set => InvokeAndValidate((ref ValueRange t) => t.limit.y = value);
		}

		/// <summary>
		/// The absolute limits of what the range may be.
		/// </summary>
		public (float min, float max) limits
		{
			get => (limit.x, limit.y);
			set => InvokeAndValidate((ref ValueRange t) => t.limit = new Vector2(value.min, value.max));
		}

		/// <summary>
		/// The value of the range, if it's operating in a fixed value mode.
		/// <see cref="isFixedValue"/> must be <see langword="true"/> to get the value.
		/// Setting the value will force the <see cref="ValueRange"/> into forced fixed mode.
		/// </summary>
		/// <exception cref="InvalidOperationException">
		/// If DEBUG is defined, thrown when the accessor is invoked while the value is not fixed.
		/// </exception>
		public float fixedValue
		{
			get
			{
				if (!isFixedValue)
				{
#if DEBUG
					throw new InvalidOperationException(
						$"{nameof(fixedValue)} cannot be read while {nameof(isFixedValue)} is false.");
#else
					return 0;
#endif // DEBUG
				}

				return minimum;
			}
			set
			{
				if (!_forceFixedValue)
				{
					// switch to force fixed
					InvokeAndValidate((ref ValueRange t) =>
					{
						t.range.x = t.range.y = value;
						t.ForceFixed();
					});

					return;
				}

				InvokeAndValidate((ref ValueRange t) =>
				{
					t.range.x = t.range.y = value;
				});
			}
		}

		/// <summary>
		/// <see langword="true"/> if the limits of the range have been modified from the defaults.
		/// </summary>
		public bool hasAssignedLimits => limit != default;

		/// <summary>
		/// <see langword="true"/> if the value range is fixed on a single value.
		/// </summary>
		public bool isFixedValue
			=> Mathf.Approximately(minimumLimit, maximumLimit) &&
			   Mathf.Approximately(minimum, maximum) &&
			   Mathf.Approximately(minimumLimit, minimum);

		private bool _forceFixedValue;

		/// <summary>
		/// <see langword="true"/> if the range is enforcing that only a fixed value can be used.
		/// Can be used to disable the forced mode by setting the value to <see langword="false" />
		/// </summary>
		public bool forceFixedValue
		{
			get => _forceFixedValue;
			set
			{
				// if we want to enable forced fixed...
				if (!_forceFixedValue && value)
				{
					ForceFixed();
				}
				else if (_forceFixedValue && !value)
				{
					// or if we want to turn it off.
					_forceFixedValue = false;
				}
			}
		}

		#region Static Instances

		// static instances of common value ranges
		private static ValueRange zeroOrEmpty = new ValueRange(0.0f, 0.0f);

		/// <summary>
		/// An empty, or zero range.
		/// </summary>
		public static ValueRange empty { get; } = zeroOrEmpty;

		/// <summary>
		/// An empty, or zero range.
		/// </summary>
		public static ValueRange zero { get; } = zeroOrEmpty;

		/// <summary>
		/// A value range from 0 to 1.
		/// </summary>
		public static ValueRange zeroOne { get; } = new ValueRange(0.0f, 1.0f);

		/// <summary>
		/// A value range from 1 to 1.
		/// </summary>
		public static ValueRange one { get; } = new ValueRange(1.0f, 1.0f);

		#endregion

		/// <summary>
		/// Creates a new <see cref="ValueRange"/>.
		/// </summary>
		/// <param name="minimumValue">The minimum value of the range.</param>
		/// <param name="maximumValue">The maximum value of the range.</param>
		/// <param name="minimumLimit">The minimum limit for the range minimum.</param>
		/// <param name="maximumLimit">The maximum limit for the range maximum.</param>
		public ValueRange(float minimumValue,
						  float maximumValue,
						  float? minimumLimit = null,
						  float? maximumLimit = null
		)
		{
			limit = new Vector2(
				minimumLimit ?? minimumValue,
				maximumLimit ?? maximumValue
			);

			range = new Vector2(
				minimumValue,
				maximumValue
			);

			_isValidating = false;
			_forceFixedValue = false;
		}

		/// <summary>
		/// Creates a <see cref="ValueRange"/> from fixed value.
		/// </summary>
		/// <param name="fixedValue">The value to assign as the fixed value and range values.</param>
		/// <returns>The new <see cref="ValueRange"/>.</returns>
		public static implicit operator ValueRange(float fixedValue)
			=> new(fixedValue, fixedValue, fixedValue, fixedValue) {_forceFixedValue = true};

		/// <summary>
		/// Creates a <see cref="ValueRange"/> from a min and max tuple.
		/// </summary>
		/// <param name="value">The min and max to create the <see cref="ValueRange"/> from.</param>
		/// <returns>The new <see cref="ValueRange"/>.</returns>
		public static implicit operator ValueRange((float Min, float Max) value)
			=> new(value.Min, value.Max, value.Min, value.Max);

		/// <summary>
		/// Forces the <see cref="ValueRange"/> into <see cref="forceFixedValue"/> mode, and returns that value.
		/// If the value isn't already fixed, will sample by the midpoint to choose as it's fixed value.
		/// </summary>
		/// <returns>The fixed value of the range.</returns>
		public float ForceFixed()
		{
			if (_forceFixedValue)
			{
				return minimum;
			}

			// switching to forced fixed, grab the current midpoint
			// and set it as min and max.
			_forceFixedValue = true;
			minimum = Midpoint();
			maximum = minimum;

			return minimum;
		}

		/// <summary>
		/// Samples the range randomly from the minimum to maximum, inclusive.
		/// </summary>
		/// <returns>A random from within the range.</returns>
		public float Random() => UnityEngine.Random.Range(minimum, maximum);

		/// <summary>
		/// Samples the range randomly from the minimum to maximum, as an <see langword="int"/>.
		/// Unlike <see cref="Random"/>, the maximum value is excluded.
		/// </summary>
		/// <returns>A random from within the range.</returns>
		public int RandomInt() => UnityEngine.Random.Range(minimumInt, maximumInt);

		/// <summary>
		/// Samples the range linearly from minimum to maximum, by the value <see cref="t"/>.
		/// </summary>
		/// <param name="t">The percentage of the range to return. (0-1)</param>
		/// <returns>
		/// The lerped value. The result is clamped to the minimum and maximum of the range.
		/// </returns>
		public float Sample(float t) => Mathf.Lerp(minimum, maximum, t);

		/// <summary>
		/// Samples the range linearly from minimum to maximum, by the value <see cref="t"/>.
		/// </summary>
		/// <param name="t">The percentage of the range to return. (0-1)</param>
		/// <returns>
		/// The lerped value. The result is unclamped and will return outside of the range if
		/// <see cref="t"/> is greater than 1 or less than 0.
		/// </returns>
		public float SampleUnclamped(float t) => Mathf.LerpUnclamped(minimum, maximum, t);

		/// <summary>
		/// Gets the minimum value of the range.
		/// </summary>
		/// <returns>The minimum value.</returns>
		public float Minimum() => minimum;

		/// <summary>
		/// Gets the midpoint value of the range.
		/// </summary>
		/// <returns>The midpoint value.</returns>
		public float Midpoint() => (minimum + maximum) / 2f;

		/// <summary>
		/// Gets the maximum value of the range.
		/// </summary>
		/// <returns>The maximum value.</returns>
		public float Maximum() => maximum;

		/// <summary>
		/// Gets the minimum value of the range, as an <see langword="int" />
		/// </summary>
		/// <returns>The minimum value.</returns>
		public int MinimumInt() => minimumInt;

		/// <summary>
		/// Gets the midpoint value of the range, as an <see langword="int" />
		/// </summary>
		/// <returns>The midpoint value.</returns>
		public int MidpointInt() => (int)((minimumInt + maximumInt) / 2f);

		/// <summary>
		/// Gets the maximum value of the range, as an <see langword="int" />
		/// </summary>
		/// <returns>The maximum value.</returns>
		public int MaximumInt() => maximumInt;

		/// <inheritdoc />
		public override string ToString()
			=> $"(Min: {minimum.ToString2Points()}, Max: {maximum.ToString2Points()})";

		/// <summary>
		/// Formats the range by the given format.
		/// </summary>
		/// <param name="format">The format to apply.</param>
		/// <returns>The formatted string.</returns>
		public string ToString(string format)
			=> ToString(format, CultureInfo.InvariantCulture.NumberFormat);

		/// <summary>
		/// Formats the range by the given format and provider.
		/// </summary>
		/// <param name="format">The format to apply.</param>
		/// <param name="formatProvider">The format provider to use.</param>
		/// <returns>The formatted string.</returns>
		public string ToString(string format, IFormatProvider formatProvider)
			=> range.ToString(format, formatProvider);

		/// <summary>
		/// Returns a string representing the value range and limits. The values are limited to two decimals,
		/// to provide a somewhat common string length.
		/// </summary>
		/// <returns>The string representing the range and limits.</returns>
		public string ToFullString()
			=> $"[{minimumLimit.ToString2Points()}] " +
			   $"{minimum.ToString2Points()} - " +
			   $"{maximum.ToString2Points()} " +
			   $"[{maximumLimit.ToString2Points()}]";

		public bool Equals(ValueRange other) => range.Equals(other.range) && limit.Equals(other.limit);

		#region ISerializationCallbackReceiver Implementation

		void ISerializationCallbackReceiver.OnBeforeSerialize() => Validate();
		void ISerializationCallbackReceiver.OnAfterDeserialize() => Validate();

		#endregion

		private void InvokeAndValidate(AssignmentAction action)
		{
			action.Invoke(ref this);
			Validate();
		}

		private void Validate()
		{
			// don't check rules if we're already checking,
			// we likely got here by enforcing one of the rules.
			if (_isValidating)
			{
				return;
			}

			_isValidating = true;

			if (_forceFixedValue)
			{
				if (!Mathf.Approximately(minimum, maximum))
				{
					// we were in forced fixed mode, but since a value has changed, we can switch back to range mode.
					_forceFixedValue = false;
				}
			}

			if (maximumLimit < minimumLimit)
			{
				maximumLimit = minimumLimit;
			}

			if (minimum < minimumLimit)
			{
				minimum = minimumLimit;
			}

			if (maximum > maximumLimit)
			{
				maximum = maximumLimit;
			}

			if (minimum > maximum)
			{
				minimum = maximum;
			}

			_isValidating = false;
		}

	}
}
