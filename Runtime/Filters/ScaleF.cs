namespace cmdwtf.UnityTools.Filters
{
	public class ScaleF : TransformBase<float>
	{
		public float Scalar { get; private set; }

		public ScaleF(float scalar)
		{
			Scalar = scalar;
		}

		public override float Sample(float sample) => sample * Scalar;

		public override void Reset() => throw new System.NotImplementedException();
	}
}
