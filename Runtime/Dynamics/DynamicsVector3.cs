using System;
using System.Collections.Generic;
using System.Drawing;

using cmdwtf.UnityTools.Attributes;

using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	[Serializable]
	public class DynamicsVector3 : IMultidimensionalDynamicsProvider, IDynamicsTransformComponent<Vector3>
	{
		[DynamicsLineColor(KnownColor.Red)]
		public SecondOrderDynamics dynamicsX = SecondOrderDynamics.Default;
		[DynamicsLineColor(KnownColor.Green)]
		public SecondOrderDynamics dynamicsY = SecondOrderDynamics.Default;
		[DynamicsLineColor(KnownColor.Blue)]
		public SecondOrderDynamics dynamicsZ = SecondOrderDynamics.Default;

		public bool modifyX = true;
		public bool modifyY = true;
		public bool modifyZ = true;

		public Vector3 Value => _value;

		protected Vector3 _value;

		public void Reset(Vector3 resetValue)
		{
			_value = resetValue;
			dynamicsX.Reset(_value.x);
			dynamicsY.Reset(_value.y);
			dynamicsZ.Reset(_value.z);
		}

		public Vector3 Update(float deltaTime, Vector3 current, Vector3 target)
		{
			_value.x = modifyX
				? dynamicsX.Update(deltaTime, target.x)
				: current.x;

			_value.y = modifyY
				? dynamicsY.Update(deltaTime, target.y)
				: current.y;

			_value.z = modifyZ
				? dynamicsZ.Update(deltaTime, target.z)
				: current.z;

			return _value;
		}

		public static implicit operator Vector3(DynamicsVector3 sov3) => sov3.Value;

		#region Implementation of IMultidimensionalDynamicsProvider

		/// <inheritdoc />
		public int DimensionCount => 3;

		/// <inheritdoc />
		public IDynamicsSystem GetDynamicsSystemForDimension(int dimensionIndex) => dimensionIndex switch
		{
			0 => dynamicsX,
			1 => dynamicsY,
			2 => dynamicsZ,
			_ => null,
		};

		/// <inheritdoc />
		public IEnumerable<IDynamicsSystem> GetDynamicsEnumerator()
		{
			for (int scan = 0; scan < DimensionCount; ++scan)
			{
				yield return GetDynamicsSystemForDimension(scan);
			}
		}

		#endregion
	}
}
