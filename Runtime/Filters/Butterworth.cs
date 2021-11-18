using System;
using SMath = System.Math;

namespace cmdwtf.UnityTools.Filters
{
	// via: https://stackoverflow.com/a/19155926
	public class Butterworth : IBiQuadFilter<float>
	{
		public const float ResonanceMax = 1.4142135623731f;
		public const float ResonanceMin = 0.1f;

		/// <inheritdoc/>
		public float Resonance { get; private set; }

		/// <inheritdoc/>
		public float Frequency { get; private set; }

		/// <inheritdoc/>
		public int SampleRate { get; private set; }

		public FilterType Type => _passType switch
		{
			ButterworthPassType.Highpass => FilterType.HighPass,
			ButterworthPassType.Lowpass => FilterType.LowPass,
			_ => FilterType.None,
		};

		private string _name;
		public virtual string Name => _name ??= GetType().Name;

		private readonly ButterworthPassType _passType;

		private readonly float _c, _a1, _a2, _a3, _b1, _b2;

		/// <summary>
		/// Array of input values, latest are in front
		/// </summary>
		private readonly float[] _inputHistory = new float[2];

		/// <summary>
		/// Array of output values, latest are in front
		/// </summary>
		private readonly float[] _outputHistory = new float[3];

		public Butterworth(float frequency, int sampleRate, ButterworthPassType passType, float resonance)
		{
			Resonance = resonance.Clamp(ResonanceMin, ResonanceMax);
			Frequency = frequency;
			SampleRate = sampleRate;
			_passType = passType;

			switch (passType)
			{
				case ButterworthPassType.Lowpass:
					_c = 1.0f / (float)SMath.Tan(SMath.PI * frequency / sampleRate);
					_a1 = 1.0f / (1.0f + (resonance * _c) + (_c * _c));
					_a2 = 2f * _a1;
					_a3 = _a1;
					_b1 = 2.0f * (1.0f - (_c * _c)) * _a1;
					_b2 = (1.0f - (resonance * _c) + (_c * _c)) * _a1;
					break;
				case ButterworthPassType.Highpass:
					_c = (float)SMath.Tan(SMath.PI * frequency / sampleRate);
					_a1 = 1.0f / (1.0f + (resonance * _c) + (_c * _c));
					_a2 = -2f * _a1;
					_a3 = _a1;
					_b1 = 2.0f * ((_c * _c) - 1.0f) * _a1;
					_b2 = (1.0f - (resonance * _c) + (_c * _c)) * _a1;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(passType), passType, null);
			}
		}

		public void Reset()
		{
			Array.Clear(_inputHistory,  0, _inputHistory.Length);
			Array.Clear(_outputHistory, 0, _outputHistory.Length);
		}

		public float Sample(float sample)
		{
			float newOutput = (_a1 * sample) +
							  (_a2 * _inputHistory[0]) +
							  (_a3 * _inputHistory[1]) -
							  (_b1 * _outputHistory[0]) -
							  (_b2 * _outputHistory[1]);

			_inputHistory[1] = _inputHistory[0];
			_inputHistory[0] = sample;

			_outputHistory[2] = _outputHistory[1];
			_outputHistory[1] = _outputHistory[0];
			_outputHistory[0] = newOutput;

			return Value;
		}

		public float Value => _outputHistory[0];
	}
}
