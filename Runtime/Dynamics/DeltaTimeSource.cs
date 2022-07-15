namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// Potential sources of getting changed time for dynamics systems.
	/// </summary>
	public enum DeltaTimeSource
	{
		Zero,
		DeltaTime,
		UnscaledDeltaTime,
		FixedUnscaledDeltaTime,
		FixedDeltaTime,
		MaximumDeltaTime,
		SmoothDeltaTime,
		MaximumParticleDeltaTime,
		Custom,
	}
}
