namespace cmdwtf.UnityTools.Filters
{
	public class NopFilter : FilterBase<float>
	{
		public override float Sample(float sample) => sample;

		public override void Reset() { }
	}
}
