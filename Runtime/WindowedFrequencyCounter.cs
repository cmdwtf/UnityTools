using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace cmdwtf.UnityTools
{
	public class WindowedFrequencyCounter : FrequencyCounterBase
	{
		private readonly Queue<float> _samples = new Queue<float>();
		private float _startTime;

		public override int Samples => _samples.Count;

		public WindowedFrequencyCounter(float period)
			: base(null, period)
		{

		}

		public WindowedFrequencyCounter(ITimeProvider timeProvider = null)
			: base(timeProvider)
		{
			_startTime = Time.NowSecondsF;
		}

		public override void Reset()
		{
			_samples.Clear();
			Frequency = 0;
			_startTime = Time.NowSecondsF;
		}

		public override void Tick()
		{
			float timestamp = Time.NowSecondsF;
			_samples.Enqueue(timestamp);
		}

		public override void Update()
		{
			float now = Time.NowSecondsF;

			// clear out samples older than the measure duration
			while (_samples.Any() && now - _samples.Peek() > Period)
			{
				_samples.Dequeue();
			}

			// calculate how long we've been holding samples
			MeasuredSeconds = System.Math.Min(now - _startTime, Period);

			// update the frequency value
			Frequency = Samples / MeasuredSeconds;
		}

		public override string ToString() => $"{Frequency.ToString2Points()} Hz";
	}
}
