using System;

using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	[AddComponentMenu("cmdwtf/Dynamics/Dynamics Transform")]
	public class DynamicsTransform : MonoBehaviour, ITransformDriver, ISerializationCallbackReceiver
	{
		[SerializeField]
		internal Transform targetTransform;
		[SerializeField]
		internal DynamicsPosition position;
		[SerializeField]
		internal DynamicsRotation rotation;
		[SerializeField]
		internal DynamicsScale scale;

		private delegate void UpdateAction(ref Transform source, ref Transform destination);

		internal const Space DefaultSpace = Space.Self;
		internal const DeltaTimeSource DefaultDtSource = DeltaTimeSource.DeltaTime;
		internal const DynamicsUpdateMode DefaultMode = DynamicsUpdateMode.LateUpdate;

		private event UpdateAction UpdateActions;
		private event UpdateAction FixedUpdateActions;
		private event UpdateAction LateUpdateActions;

		private bool _dynamicsResetNeeded;

		public void SetTarget(Transform newTarget)
		{
			if (newTarget == targetTransform)
			{
				return;
			}

			targetTransform = newTarget;

			OnTargetAttached();
		}

		public void SetCustomDeltaTimeSource(Func<float> getDeltaTime)
		{
			position.CustomDeltaTimeSource = getDeltaTime;
			rotation.CustomDeltaTimeSource = getDeltaTime;
			scale.CustomDeltaTimeSource = getDeltaTime;
		}

		private void OnTargetAttached()
		{
			if (targetTransform == null)
			{
				return;
			}

			if (targetTransform.IsChildOf(transform))
			{
				// #todo: handle target being a child
				Debug.LogWarning($"{nameof(DynamicsTransform)} does not currently have support for following " +
								 $"a transform that belongs to a child object.");
				enabled = false;
			}
		}

		private void Start()
		{
			if (_dynamicsResetNeeded)
			{
				ResetDynamics();
			}
		}

		private void OnEnable()
		{
			if (targetTransform == null)
			{
				Debug.LogWarning($"{gameObject.name}'s {name} expects a target transform. This component does nothing without one! Will disable.");
				enabled = false;
				return;
			}

			OnTargetAttached();
		}

		private void Reset()
		{
			targetTransform = null;

			position = new DynamicsPosition()
			{
				enabled = true,
				space = DefaultSpace,
				mode = DefaultMode,
				deltaTimeSource = DefaultDtSource,
			};

			position.Mutator.Reset(Vector3.zero);

			rotation = new DynamicsRotation()
			{
				enabled = true,
				space = DefaultSpace,
				mode = DefaultMode,
				deltaTimeSource = DefaultDtSource,
			};

			rotation.Mutator.Reset(Quaternion.identity);

			scale = new DynamicsScale()
			{
				enabled = true,
				space = DefaultSpace,
				mode = DefaultMode,
				deltaTimeSource = DefaultDtSource,
			};

			scale.Mutator.Reset(Vector3.zero);

			ReassignEvents();
		}

		private void Update()
		{
			if (_dynamicsResetNeeded)
			{
				ResetDynamics();
			}

			InvokeUpdates(UpdateActions);
		}

		private void FixedUpdate() => InvokeUpdates(FixedUpdateActions);

		private void LateUpdate() => InvokeUpdates(LateUpdateActions);

		private void InvokeUpdates(UpdateAction actions)
		{
			if (actions == null)
			{
				return;
			}

			Transform self = transform;
			actions.Invoke(ref targetTransform, ref self);
		}

		private void ReassignEvents()
		{
			UpdateActions = null;
			FixedUpdateActions = null;
			LateUpdateActions = null;

			AddModifyEventByMode(position.Update, position.mode);
			AddModifyEventByMode(rotation.Update, rotation.mode);
			AddModifyEventByMode(scale.Update, scale.mode);
		}

		private void ResetDynamics()
		{
			_dynamicsResetNeeded = false;

			Transform t = targetTransform ? targetTransform : transform;

			OnTargetAttached();

			position.Reset(ref t);
			rotation.Reset(ref t);
			scale.Reset(ref t);
		}

		private void AddModifyEventByMode(UpdateAction ev, DynamicsUpdateMode mode)
		{
			switch (mode)
			{
				case DynamicsUpdateMode.CopyOnUpdate:
				case DynamicsUpdateMode.Update:
					UpdateActions += ev;
					break;
				case DynamicsUpdateMode.FixedUpdate:
					FixedUpdateActions += ev;
					break;
				case DynamicsUpdateMode.LateUpdate:
					LateUpdateActions += ev;
					break;
			}
		}

		#region ISerializationCallbackReceiver Implementation

		/// <inheritdoc />
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			// noop
		}

		/// <inheritdoc />
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			ReassignEvents();
			_dynamicsResetNeeded = true;
		}

		#endregion

		#region ITransformDriver Implementation

		/// <inheritdoc />
		public bool CanDrivePosition => IsDrivingPosition || position.mode != DynamicsUpdateMode.Ignored;

		/// <inheritdoc />
		public bool CanDriveRotation => IsDrivingRotation || rotation.mode != DynamicsUpdateMode.Ignored;

		/// <inheritdoc />
		public bool CanDriveScale => IsDrivingScale || scale.mode != DynamicsUpdateMode.Ignored;

		/// <inheritdoc />
		public bool IsDrivingPosition
		{
			get => position?.enabled ?? false;
			set
			{
				if (position != null)
				{
					position.enabled = value;
				}
			}
		}

		/// <inheritdoc />
		public bool IsDrivingRotation
		{
			get => rotation?.enabled ?? false;
			set
			{
				if (rotation != null)
				{
					rotation.enabled = value;
				}
			}
		}

		/// <inheritdoc />
		public bool IsDrivingScale
		{
			get => scale?.enabled ?? false;
			set
			{
				if (scale != null)
				{
					scale.enabled = value;
				}
			}
		}

		/// <inheritdoc />
		public bool IsDrivingAnyTransformComponent
			=> IsDrivingPosition ||
			   IsDrivingRotation ||
			   IsDrivingScale;

		#endregion
	}
}
