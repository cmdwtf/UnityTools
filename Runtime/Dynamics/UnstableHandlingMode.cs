using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// The method a dynamics system should use to handle when it is in an unstable state.
	/// </summary>
	public enum UnstableHandlingMode
	{
		[Tooltip("When the system is unstable, update will return the given target value.")]
		[InspectorName("Return Target Value")]
		ReturnTarget,
		[Tooltip("When the system is unstable, update will return the value 0.")]
		[InspectorName("Return 0")]
		Return0,
		[Tooltip("Return Last Stable Value")]
		[InspectorName("When the system is unstable, update will return the last stable value the system produced.")]
		ReturnLastStableValue,
		[Tooltip("When the system is unstable, it will allow the non-finite, non-zero values to be returned as they are generated.")]
		[InspectorName("Allow Denormalized Values")]
		AllowDenormalizedValues,
	}
}
