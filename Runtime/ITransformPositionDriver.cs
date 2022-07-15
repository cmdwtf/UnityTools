namespace cmdwtf.UnityTools
{
	/// <summary>
	/// Describes a type that can drive a transform position.
	/// </summary>
	public interface ITransformPositionDriver : ITransformComponentDriver
	{
		bool CanDrivePosition { get; }
		bool IsDrivingPosition { get; set; }
	}
}
