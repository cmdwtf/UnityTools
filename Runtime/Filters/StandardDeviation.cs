using System.Linq;

namespace cmdwtf.UnityTools.Filters
{
	public class StandardDeviation : SimpleMovingAverage
	{
		protected float Average { get; private set; }

		public float Sigma { get; private set; }

		public StandardDeviationType StandardDeviationType { get; }

		public StandardDeviation(int windowSize, StandardDeviationType type = StandardDeviationType.Population)
			: base(windowSize)
		{
			StandardDeviationType = type;
		}

		public override float Sample(float sample)
		{
			Average = base.Sample(sample);
			float varianceSquaredSum = Samples.Sum(d => (float)System.Math.Pow(d - Average, 2));
			float divisor = StandardDeviationType == StandardDeviationType.Population
								? SampleCount
								: SampleCount - 1;
			float variance = varianceSquaredSum / divisor;
			Sigma = (float)System.Math.Sqrt(variance);
			Value = Sigma;
			return Value;
		}

		public float GetStandardDeviationsFromMean(float sample)
		{
			if (SampleCount <= 0 || Sigma == 0)
			{
				return 0;
			}

			float result = (sample - Average) / Sigma;
			return result;
		}

		public override void Reset()
		{
			base.Reset();
			Sigma = default;
		}
	}
}
