using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// Modes that a dynamic system may use to update objects.
	/// </summary>
	public enum DynamicsUpdateMode
	{
		[InspectorName("Do Not Modify")]
		[Tooltip("The selected value will not be modified.")]
		Ignored,

		[InspectorName("Copy target value on Update()")]
		[Tooltip("The selected value will not be modified, and instead just copied from the target on Update().")]
		CopyOnUpdate,

		[InspectorName("Modify on Update()")]
		[Tooltip("The target transform will be used to update the system on the Update() message.")]
		Update,

		[InspectorName("Modify on FixedUpdate()")]
		[Tooltip("The target transform will be used to update the system on the FixedUpdate() message.")]
		FixedUpdate,

		[InspectorName("Modify on LateUpdate()")]
		[Tooltip("The target transform will be used to update the system on the LateUpdate() message.")]
		LateUpdate,
	}
}
