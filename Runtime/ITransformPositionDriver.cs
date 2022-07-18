namespace cmdwtf.UnityTools
{
	/// <summary>
	/// Describes a type that can drive a transform position.
	/// </summary>
	public interface ITransformPositionDriver : ITransformComponentDriver
	{
		/// <summary>
		/// Returns true if the object can drive a transform's position.
		/// </summary>
		bool CanDrivePosition { get; }

		/// <summary>
		/// Returns true if the object is driving a transform's position.
		/// </summary>
		bool IsDrivingPosition { get; set; }
	}
}
