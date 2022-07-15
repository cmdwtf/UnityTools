namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// An interface that describes a type that implements a dynamics system.
	/// </summary>
	public interface IDynamicsSystem
	{
		/// <summary>
		/// Resets the system.
		/// </summary>
		void Reset();

		/// <summary>
		/// The current value of the system.
		/// </summary>
		float Value { get; }
	}
}
