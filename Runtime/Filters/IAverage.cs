namespace cmdwtf.UnityTools.Filters
{
	public interface IAverage<T> : IFilter<T>
	{
		int WindowSize { get; }
	}
}
