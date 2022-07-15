namespace cmdwtf.UnityTools
{
	/// <summary>
	/// Describes a type that can drive one or more transform components.
	/// </summary>
	public interface ITransformDriver
		: ITransformPositionDriver, ITransformRotationDriver, ITransformScaleDriver
	{
		bool IsDrivingAnyTransformComponent { get; }
	}
}
