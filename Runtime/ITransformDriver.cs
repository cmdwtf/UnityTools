namespace cmdwtf.UnityTools
{
	/// <summary>
	/// Describes a type that can drive one or more transform components.
	/// </summary>
	public interface ITransformDriver
		: ITransformPositionDriver, ITransformRotationDriver, ITransformScaleDriver
	{
		/// <summary>
		/// Returns true if the object is driving any component of a transform's.
		/// </summary>
		bool IsDrivingAnyTransformComponent { get; }
	}
}
