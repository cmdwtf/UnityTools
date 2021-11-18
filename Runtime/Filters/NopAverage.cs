namespace cmdwtf.UnityTools.Filters
{
	public class NopAverage : AverageBase<float>
	{
		public NopAverage() : base(0) { }

		public override float Sample(float sample) => Value = sample;
	}
}
