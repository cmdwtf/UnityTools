namespace cmdwtf.UnityTools.Filters
{
	public class WeightedMovingAverageStrong : AverageBase<float>
	{
		public WeightedMovingAverageStrong(int windowSize)
			: base(windowSize) { }

		public override float Sample(float sample)
		{
			AppendSample(sample);
			float accumulator = 0;
			int scan = 1;
			float divisor = SampleCount.NthTriangular();
			foreach (float s in Samples)
			{
				accumulator += s * scan / divisor;
				scan++;
			}

			Value = accumulator;
			return Value;
		}
	}
}
