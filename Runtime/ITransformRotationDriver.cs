namespace cmdwtf.UnityTools
{
	/// <summary>
	/// Describes a type that can drive a transform rotation.
	/// </summary>
	public interface ITransformRotationDriver : ITransformComponentDriver
	{
		/// <summary>
		/// Returns true if the object can drive a transform's rotation.
		/// </summary>
		bool CanDriveRotation { get; }

		/// <summary>
		/// Returns true if the object is driving a transform's rotation.
		/// </summary>
		bool IsDrivingRotation { get; set; }
	}
}
