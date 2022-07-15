using System;

using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	[Serializable]
	public sealed class DynamicsRotation
		: DynamicsTransformMutator<DynamicsQuaternion, Quaternion>
	{
		#region Overrides of DynamicsTransformMutator<DynamicsQuaternion,Quaternion>

		/// <inheritdoc />
		protected override Quaternion GetSourceValue(ref Transform source)
			=> space switch
			{
				Space.Self => source.localRotation,
				_          => source.rotation,
			};

		/// <inheritdoc />
		protected override void SetDestinationValue(ref Transform destination, Quaternion value)
		{
			if (space == Space.Self)
			{
				destination.localRotation = value;
			}
			else
			{
				destination.rotation = value;
			}
		}

		#endregion
	}
}
