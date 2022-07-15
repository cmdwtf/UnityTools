using System;

using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	[Serializable]
	public sealed class DynamicsPosition
		: DynamicsTransformMutator<DynamicsVector3, Vector3>
	{
		#region Overrides of DynamicsTransformMutator<DynamicsVector3,Vector3>

		/// <inheritdoc />
		protected override Vector3 GetSourceValue(ref Transform source)
			=> space switch
			{
				Space.Self => source.localPosition,
				_          => source.position,
			};

		/// <inheritdoc />
		protected override void SetDestinationValue(ref Transform destination, Vector3 value)
		{
			if (space == Space.Self)
			{
				destination.localPosition = value;
			}
			else
			{
				destination.position = value;
			}
		}

		#endregion
	}
}
