using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// Different methods of simulating dynamics on rotation quaternions.
	/// </summary>
	public enum DynamicsQuaternionRotationType
	{
		[InspectorName("Quaternion")]
		[Tooltip("This will simulate the dynamics of each component of the quaternion individually.")]
		QuaternionComponents,

		[InspectorName("Forward and Upwards")]
		[Tooltip("This simulates dynamics for two individual vectors, one representing the rotation's forward, " +
		         "one representing the rotation's up. Each of those can be configured individually.")]
		ForwardAndUpward,
	}
}
