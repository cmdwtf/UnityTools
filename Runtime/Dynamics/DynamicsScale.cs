using System;

using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	[Serializable]
	public sealed class DynamicsScale
		: DynamicsTransformMutator<DynamicsVector3, Vector3>
	{
		#region Overrides of DynamicsTransformMutator<DynamicsVector3,Vector3>

		/// <inheritdoc />
		protected override Vector3 GetSourceValue(ref Transform source)
			=> space switch
			{
				Space.Self => source.localScale,
				_          => source.lossyScale,
			};

		/// <inheritdoc />
		protected override void SetDestinationValue(ref Transform destination, Vector3 value)
			=> destination.localScale = value;

		#endregion
	}
}
