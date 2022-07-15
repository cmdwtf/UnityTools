namespace cmdwtf.UnityTools
{
	/// <summary>
	/// Describes a type that can drive a transform scale.
	/// </summary>
	public interface ITransformScaleDriver : ITransformComponentDriver
	{
		bool CanDriveScale { get; }
		bool IsDrivingScale { get; set; }
	}
}
