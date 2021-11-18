namespace cmdwtf.UnityTools.Filters
{
	public class StandardDeviationsFromMean : StandardDeviation
	{
		public StandardDeviationsFromMean(int windowSize, StandardDeviationType type = StandardDeviationType.Population)
			: base(windowSize, type) { }

		public override float Sample(float sample) => SampleCurrent(sample);

		public float SamplePrevious(float sample)
		{
			// get value from previous sample's stddev
			float stdDevs = GetStandardDeviationsFromMean(sample);

			// update stddev,
			base.Sample(sample);

			// return the previously calculated value, after saving it
			Value = stdDevs;

			return Value;
		}

		public float SampleCurrent(float sample)
        {
        	// update stddev,
        	base.Sample(sample);

        	// get value from current sample's stddev
        	float stdDevs = GetStandardDeviationsFromMean(sample);

        	// return the previously calculated value, after saving it
        	Value = stdDevs;
			return Value;
        }
	}
}
