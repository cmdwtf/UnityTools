namespace cmdwtf.UnityTools
{
	/// <summary>
	/// Describes a type that can drive a transform rotation.
	/// </summary>
	public interface ITransformRotationDriver : ITransformComponentDriver
	{
		bool CanDriveRotation { get; }
		bool IsDrivingRotation { get; set; }
	}
}
