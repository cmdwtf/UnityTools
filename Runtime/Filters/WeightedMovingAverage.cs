namespace cmdwtf.UnityTools.Filters
{
	public class WeightedMovingAverage : AverageBase<float>
	{
		public WeightedMovingAverage(int windowSize)
			: base(windowSize)
		{

		}

		public override float Sample(float sample)
		{
			AppendSample(sample);
			float accumulator = 0;
			int scan = 1;
			foreach (float s in Samples)
			{
				accumulator += (s * scan) / SampleCount.NthTriangular();
				scan++;
			}

			Value = accumulator;
			return Value;
		}
	}
}
