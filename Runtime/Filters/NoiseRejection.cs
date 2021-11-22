using System.Collections.Generic;

namespace cmdwtf.UnityTools.Filters
{
	public class NoiseRejection : StandardDeviationsFromMean
	{
		public int RejectionWindowSize { get; protected set; }
		public float RejectionThreshold { get; protected set; }

		private float PreviousSample { get; set; }

		private const int MinimumStandardDeviationWindowSize = 10;

		public NoiseRejection(int windowSize,
							  int rejectionWindowSize,
							  float rejectionThreshold,
							  StandardDeviationType type = StandardDeviationType.Population
		)
			: base(windowSize.Clamp(MinimumStandardDeviationWindowSize, int.MaxValue), type)
		{
			RejectionWindowSize = rejectionWindowSize;
			RejectionThreshold = rejectionThreshold;
		}

		private readonly Queue<float> _previousFromMeans = new Queue<float>();
		private readonly Queue<float> _previousValues = new Queue<float>();

		public override float Sample(float sample)
		{
			float fromMean = base.Sample(sample).Positive();

			if (!IsValidDeviation(fromMean))
			{
				// might be a bad sample, check previous means.
				// if all of them are 'invalid', assume the deviation
				// level we are at is expected, and let the sample through
				foreach (float previousFromMean in _previousFromMeans)
				{
					if (IsValidDeviation(previousFromMean))
					{
						// found a valid one, reject the sample
						UndoLastSampleAppend();

						// use some of it's values, but weight it down with the average of the previous
						//float previousAverages = _previousValues.SimpleAverage();
						//sample = (sample + (previousAverages * _previousValues.Count)) / (_previousValues.Count + 1);
						sample = PreviousSample;

						break;
					}
				}
			}

			// sample is either good or corrected now!
			_previousFromMeans.Enqueue(fromMean);
			_previousValues.Enqueue(sample);

			while (_previousValues.Count > RejectionWindowSize)
			{
				_previousValues.Dequeue();
				_previousFromMeans.Dequeue();
			}

			Value = sample;
			PreviousSample = sample;

			return Value;
		}

		private bool IsValidDeviation(float stdDevsFromMean) => stdDevsFromMean < RejectionThreshold;
	}
}
