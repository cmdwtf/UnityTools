using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// The mode a dynamics system should use when sampling from <see cref="ValueRange"/>s.
	/// </summary>
	public enum SamplingMode
	{
		[Tooltip("Will return 0 when sampled.")]
		[InspectorName("None (0)")]
		None,
		[Tooltip("Will return the given value when sampled.")]
		[InspectorName("Fixed Input Values")]
		Fixed,
		[Tooltip("Will return the minimum of the range when sampled.")]
		[InspectorName("Minimum from Value Range")]
		Minimum,
		[Tooltip("Will return the midpoint of the range when sampled.")]
		[InspectorName("Midpoint from Value Range")]
		Midpoint,
		[Tooltip("Will return the maximum of the range when sampled.")]
		[InspectorName("Maximum from Value Range")]
		Maximum,
		[Tooltip("Will return the a random value within the range when sampled.")]
		[InspectorName("Random from within Value Range")]
		Random,
		//[Tooltip("Will return the a value from the range where the interpolation factor is based on the step time, " +
		//         "when sampled. The upper limit of the linear interpolation is based on the 'lerpMaxTime' setting.")]
		//[InspectorName("Step Time (ΔT) Based")]
		//LerpByStepTime,
		//[Tooltip("Will return the a value from the range where the interpolation factor " +
		//         "is based on the time since the system was last reset, when sampled. The upper limit of the " +
		//         "linear interpolation is based on the 'lerpMaxTime' setting.")]
		//[InspectorName("Elapsed Time Based")]
		//LerpByScaledTime,
	}
}
