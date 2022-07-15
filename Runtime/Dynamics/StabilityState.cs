using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// The current stability of a given dynamics system.
	/// </summary>
	public enum StabilityState
	{
		[Tooltip("The system is stable.")]
		Stable,

		[Tooltip("The system's current value is a denormalized number and should likely not be used.")]
		DenormalizedValue,

		[Tooltip("The system's current value exceeds it's max limit.")]
		BeyondMaximumLimit,

		[Tooltip("The system has an input value that will cause it to never approach its target.")]
		NeverApproachingTarget,
	}
}
