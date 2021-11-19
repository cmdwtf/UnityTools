namespace cmdwtf.UnityTools.Filters
{
	public abstract class TransformBase<T> : FilterBase<T>
	{
		public override FilterType Type => FilterType.Transformation;
	}
}
