namespace cmdwtf.UnityTools.Filters
{
	public abstract class FilterBase<T> : IFilter<T>
	{
		public virtual FilterType Type => FilterType.None;

		private string _name;
		public virtual string Name => _name ??= GetType().Name;

		public virtual T Value { get; protected set; }

		public abstract T Sample(T sample);

		public abstract void Reset();
	}
}
