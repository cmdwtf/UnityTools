using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

using cmdwtf.UnityTools.Attributes;

using UnityEngine;
using UnityEngine.Serialization;

namespace cmdwtf.UnityTools.Dynamics
{
	[Serializable]
	public class DynamicsQuaternion : IMultidimensionalDynamicsProvider, IDynamicsTransformComponent<Quaternion>, ISerializationCallbackReceiver
	{
		[FormerlySerializedAs("rotationMode")]
		public DynamicsQuaternionRotationType rotationType = DynamicsQuaternionRotationType.QuaternionComponents;

		[DynamicsLineColor(KnownColor.Red)]
		public SecondOrderDynamics dynamicsX = SecondOrderDynamics.Default;
		[DynamicsLineColor(KnownColor.Green)]
		public SecondOrderDynamics dynamicsY = SecondOrderDynamics.Default;
		[DynamicsLineColor(KnownColor.Blue)]
		public SecondOrderDynamics dynamicsZ = SecondOrderDynamics.Default;
		[DynamicsLineColor(KnownColor.Purple)]
		public SecondOrderDynamics dynamicsW = SecondOrderDynamics.Default;
		public DynamicsVector3 dynamicsUpwards = new();
		public DynamicsVector3 dynamicsForward = new();

		public Quaternion Value => _value;
		private Quaternion _value;

		private Vector3 _valueForward;
		private Vector3 _valueUpwards;

		private bool _dynamicsResetNeeded;

		public void Reset(Quaternion resetValue)
		{
			// store the reset value
			_value = resetValue;
			dynamicsX.Reset(resetValue.x);
			dynamicsX.Reset(resetValue.y);
			dynamicsX.Reset(resetValue.z);
			dynamicsW.Reset(resetValue.w);

			// store our up/fwd and reset their dynamics too.
			_valueForward = resetValue * Vector3.forward;
			dynamicsForward.Reset(_valueForward);

			_valueUpwards = resetValue * Vector3.up;
			dynamicsUpwards.Reset(_valueUpwards);
		}

		public Quaternion Update(float deltaTime, Quaternion current, Quaternion target)
		{
			if (_dynamicsResetNeeded)
			{
				ResetDynamics();
			}

			switch (rotationType)
			{
				case DynamicsQuaternionRotationType.QuaternionComponents:
					UpdateQuaternion(deltaTime, target);
					break;
				case DynamicsQuaternionRotationType.ForwardAndUpward:
					UpdateForwardUp(deltaTime, target);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return Value;
		}

		private void UpdateQuaternion(float deltaTime, Quaternion target)
		{
			_value.x = dynamicsX.Update(deltaTime, target.x);
			_value.y = dynamicsY.Update(deltaTime, target.y);
			_value.z = dynamicsZ.Update(deltaTime, target.z);
			_value.w = dynamicsW.Update(deltaTime, target.w);

			// store our new q as up/fwd
			_valueUpwards = _value * Vector3.up;
			_valueForward = _value * Vector3.forward;
		}

		private void UpdateForwardUp(float deltaTime, Quaternion target)
		{
			Vector3 targetForward = target * Vector3.forward;
			Vector3 targetUpwards = target * Vector3.up;

			Vector3 currentForward = _value * Vector3.forward;
			Vector3 currentUpwards = _value * Vector3.up;

			_valueForward = dynamicsForward.Update(deltaTime, currentForward, targetForward);
			_valueUpwards = dynamicsUpwards.Update(deltaTime, currentUpwards, targetUpwards);

			// store our new up/fwd as q
			_value = Quaternion.LookRotation(_valueForward, _valueUpwards);
		}

		private void ResetDynamics()
		{
			_dynamicsResetNeeded = false;
			Reset(_value);
		}

		public static implicit operator Quaternion(DynamicsQuaternion soq) => soq.Value;

		#region ISerializationCallbackReceiver Implementation

		/// <inheritdoc />
		void ISerializationCallbackReceiver.OnBeforeSerialize() { }

		/// <inheritdoc />
		void ISerializationCallbackReceiver.OnAfterDeserialize() => _dynamicsResetNeeded = true;

		#endregion

		#region Implementation of IMultidimensionalDynamicsProvider

		/// <inheritdoc />
		public int DimensionCount => 4;

		/// <inheritdoc />
		public IDynamicsSystem GetDynamicsSystemForDimension(int dimensionIndex) => dimensionIndex switch
		{
			0 => dynamicsX,
			1 => dynamicsY,
			2 => dynamicsZ,
			3 => dynamicsW,
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
