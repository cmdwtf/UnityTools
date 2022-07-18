namespace cmdwtf.UnityTools
{
	/// <summary>
	/// Describes a type that can drive a transform scale.
	/// </summary>
	public interface ITransformScaleDriver : ITransformComponentDriver
	{
		/// <summary>
		/// Returns true if the object can drive a transform's scale.
		/// </summary>
		bool CanDriveScale { get; }

		/// <summary>
		/// Returns true if the object is driving a transform's scale.
		/// </summary>
		bool IsDrivingScale { get; set; }
	}
}
