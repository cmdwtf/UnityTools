using UnityEngine;

namespace cmdwtf.UnityTools
{
	/// <summary>
	/// A component that will blend a camera between perspective and orthographic modes.
	/// </summary>
	[RequireComponent(typeof(Camera))]
	[HelpURL("https://cmd.wtf")]
	public class CameraProjectionBlender : MonoBehaviour
	{
		[Header("Blender Settings")]
		[Tooltip("The amount of time the camera should interpolate between modes if a custom animation curve is not provided.")]
		public float defaultBlendDuration = 4f;

		[Header("Perspective Settings")]
		[Tooltip("The vertical field of view (in degrees) the camera should have in perspective mode.")]
		[Range(0.00001f, 180.0f - 0.00001f)]
		public float fieldOfView = 60f;

		[Header("Orthographic Settings")]
		[Tooltip("The size the the camera should have in orthographic mode.")]
		public float orthographicSize = 4f;

		private bool BlendNeedsUpdate
			=> (_blendCurve != null) ||
			   (!Mathf.Approximately(_lastBlendValue, _blendValue)) ||
			   (_camera.orthographic
				   ? !Mathf.Approximately(_camera.orthographicSize, orthographicSize)
				   : !Mathf.Approximately(_camera.fieldOfView, _lastFieldOfView));
		internal bool FinishedOrthographic => _lastBlendValue >= BlendFullOrthographic;
		internal bool FinishedPerspective => _lastBlendValue <= BlendFullPerspective;

		private Camera _camera;

		private float _elapsed;
		private AnimationCurve _blendCurve;
		private float _blendValue;
		private float _lastBlendValue;
		private float _lastFieldOfView;
		private float _lastSize;
		private bool _cancelBlend;

		private Vector3 _initialLocalPosition;
		private Matrix4x4 _initialProjection;
		private FrustumPlanes _initialFrustum;
		private bool _initialIsOrthographic;
		private float _initialNearClip;
		private float _initialFarClip;

		private const float RepeatedUpdateDelta = 1 / 60.0f;
		private const float BlendMinimum = 0.0f;
		private const float BlendMaximum = 1.0f;
		private const float BlendFullPerspective = BlendMinimum;
		private const float BlendFullOrthographic = BlendMaximum;

		// keep large distances away, things get weird with big numbers.
		private const float FovDistanceMaximum = 100000.0f;

		// Orthographic is essentially a projection camera with a 0 degree field of view and infinite distance.
		private const float TargetOrthographicFieldOfView = 0;
		private const float TargetOrthographicDistance = float.PositiveInfinity;

		// Projection is essentially an orthographic camera with a 0 size.
		private const float TargetPerspectiveSize = 0;
		private const float TargetPerspectiveDistance = 0;

		// The actual orthographic size is 2 * Camera.orthographicSize,
		// So we need our target size to reflect that scale.
		private float TargetOrthographicSize => orthographicSize * 2.0f;


		private float InitialFrustumDepth => _initialFrustum.zFar - _initialFrustum.zNear;
		private float InitialFrustumMidpoint => InitialFrustumDepth / 2.0f;

		private float _initialAspect;

		/// <summary>
		/// Calculates a distance based on a size and field of view.
		/// </summary>
		/// <param name="size">The size of the camera.</param>
		/// <param name="fov">The camera's field of view.</param>
		/// <returns>The distance away the camera should be.</returns>
		private static float DistanceFromFieldOfViewAndSize(float size, float fov)
			=> size / (2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * fov));

		/// <summary>
		/// Gets a camera size value based on a distance and field of view.
		/// </summary>
		/// <param name="distance">The distance away the camera is.</param>
		/// <param name="fov">The camera's field of view.</param>
		/// <returns>Teh size value.</returns>
		private static float SizeFromDistanceAndFieldOfView(float distance, float fov)
			=> 2.0f * distance * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);

		/// <summary>
		/// Calculates the field of view needed to get a given frustum size at a given distance.
		/// </summary>
		/// <param name="size">The size of the camera.</param>
		/// <param name="distance">The distance away the camera is.</param>
		/// <returns>The field of view.</returns>
		private static float FieldOfViewFromSizeAndDistance(float size, float distance)
			=> 2.0f * Mathf.Atan(size * 0.5f / distance) * Mathf.Rad2Deg;

		private void Reset()
		{
			UpdateInitialFields();

			if (!_initialIsOrthographic)
			{
				fieldOfView = _camera.fieldOfView;
			}
		}

		private void Awake()
		{
			UpdateInitialFields();
		}

		private void LateUpdate()
		{
			UpdateProjectionBlend(Time.deltaTime, forceValidDistance: false);
		}

		internal void UpdateProjectionBlend(float delta, bool forceValidDistance)
		{
			// no sense in doing math to waste cycles
			if (!BlendNeedsUpdate)
			{
				return;
			}

			bool distanceValid = false;
			float distance = 0;

			// update our blend values and interpolate our size/fov
			// repeating if needed to get a distance under the maximum.
			while (!distanceValid)
			{
				// update our blend ('t') amount.
				UpdateBlendValue(delta);

				// any further blend updates should use our repeated delta.
				delta = RepeatedUpdateDelta;

				// get the interpolated camera 'sizes' and interpolated field of view.
				_lastSize = Mathf.Lerp(TargetPerspectiveSize, TargetOrthographicSize, _lastBlendValue);
				_lastFieldOfView = Mathf.Lerp(fieldOfView, TargetOrthographicFieldOfView, _lastBlendValue);

				// move the camera back to keep the focal point the same as our FOV changes.
				distance = DistanceFromFieldOfViewAndSize(_lastSize, _lastFieldOfView);

				// check for invalid distances,
				// which we can use to exit early, if allowed
				if (!forceValidDistance)
				{
					if (float.IsInfinity(distance) || float.IsSubnormal(distance))
					{
						break;
					}
				}

				// if we have a massive distance,
				// we wil throw it out and update once more.
				if (Mathf.Abs(distance) >= FovDistanceMaximum)
				{
					continue;
				}

				// once here, a zero or other normal number is valid!
				if (distance == 0 || float.IsNormal(distance))
				{
					distanceValid = true;
				}
			}

			UpdateCamera(distance, distanceValid);
		}

		private void UpdateCamera(float distance, bool distanceValid)
		{
			// handle if a blend was cancelled.
			CheckCancelBlend();
			UpdateCameraDistance(distance, distanceValid);
			UpdateCameraProjection(distance);
		}

		private void UpdateCameraDistance(float distance, bool distanceValid)
		{
			// offset the local position by moving the initial position backwards
			Vector3 backwards = (transform.forward * -1);
			Vector3 offset = distanceValid
									 ? backwards * distance
									 : backwards * InitialFrustumMidpoint;

			transform.localPosition = (_initialLocalPosition + offset);
		}

		private void UpdateCameraProjection(float distance)
		{
			// check to switch the camera into full orthographic mode if we've reached that point of the blend.
			if (FinishedOrthographic)
			{
				// switch to ortho mode if we aren't already.
				if (!_camera.orthographic)
				{
					_camera.orthographic = true;
				}

				_camera.nearClipPlane = _initialNearClip;
				_camera.farClipPlane = _initialFarClip;

				float orthographicWidth = orthographicSize * _initialAspect;
				_camera.orthographicSize = orthographicSize;

				_camera.projectionMatrix = Matrix4x4.Ortho(-orthographicWidth,
														   orthographicWidth,
														   -orthographicSize,
														   orthographicSize,
														   _initialNearClip,
														   _initialFarClip);
			}
			else if (!FinishedOrthographic)
			{
				// otherwise update the camera in it's perspective mode.

				// disable orthographic mode if it was enabled.
				if (_camera.orthographic)
				{
					_camera.orthographic = false;
				}

				// offset the clip distance if we've moved a half a frustum
				// from where we started in full perspective, otherwise
				// at extreme distances, (very very close to orthographic),
				// some things may be culled.
				float clipOffset = distance >= InitialFrustumMidpoint
									   ? distance - InitialFrustumMidpoint
									   : 0;

				float clipNear = _initialNearClip + clipOffset;
				float clipFar = _initialFarClip + clipOffset;

				_camera.nearClipPlane = clipNear;
				_camera.farClipPlane = clipFar;

				_camera.projectionMatrix = Matrix4x4.Perspective(_lastFieldOfView,
																 _initialAspect,
																 clipNear,
																 clipFar);
			}
		}

		private void CheckCancelBlend()
		{
			if (_cancelBlend)
			{
				_blendCurve = null;
				_cancelBlend = false;
			}
		}

		private void UpdateBlendValue(float delta)
		{
			if (_blendCurve != null)
			{
				// update time that has passed,
				_elapsed += delta;

				Keyframe last = _blendCurve[_blendCurve.length - 1];

				// toss our curve when we're done with it.
				if (_elapsed >= last.time)
				{
					_elapsed = last.time;
					_blendValue = last.value;
					_blendCurve = null;
				}
				else
				{
					// update our blend amount, keeping within expected range.
					_blendValue = _blendCurve.Evaluate(_elapsed);
				}
			}

			_blendValue = Mathf.Clamp(_blendValue, BlendMinimum, BlendMaximum);
			_lastBlendValue = _blendValue;
		}

		/// <summary>
		/// Gathers the camera on this game object,
		/// and stores all of the initial values used to blend between.
		/// </summary>
		private void UpdateInitialFields()
		{
			_camera = GetComponent<Camera>();
			_initialProjection = _camera.projectionMatrix;

			// don't use the 'Screen' variables from functions while in editor gui...
			// you'll get some numbers you don't expect.
			_initialAspect = Screen.width / (float)Screen.height;

			_initialNearClip = _camera.nearClipPlane;
			_initialFarClip = _camera.farClipPlane;

			_initialLocalPosition = transform.localPosition;
			_initialFrustum = _initialProjection.decomposeProjection;

			_initialIsOrthographic = _camera.orthographic;

			if (_initialIsOrthographic)
			{
				_blendValue = BlendFullOrthographic;
				_lastBlendValue = BlendFullOrthographic;
			}
			else
			{
				_lastFieldOfView = _camera.fieldOfView;
			}
		}

		private void SetBlendTarget(float targetBlendValue, AnimationCurve ani = null)
		{
			float deltaPercent = Mathf.Abs(_blendValue - targetBlendValue);

			// if we aren't changing our target, there's no need to do anything.
			if (deltaPercent == 0)
			{
				return;
			}

			_blendCurve = ani ?? AnimationCurve.EaseInOut(
							  0,
							  _lastBlendValue,
							  defaultBlendDuration * deltaPercent,
							  targetBlendValue);
			_elapsed = 0;
		}

		/// <summary>
		/// Sets the camera to blend to the perspective state.
		/// </summary>
		/// <param name="ani">An optional animation curve to follow for the blend.</param>
		public void Perspective(AnimationCurve ani = null)
		{
			SetBlendTarget(BlendFullPerspective, ani);
		}

		/// <summary>
		/// Sets the camera to blend to the orthographic state.
		/// </summary>
		/// <param name="ani">An optional animation curve to follow for the blend.</param>
		public void Orthographic(AnimationCurve ani = null)
		{
			SetBlendTarget(BlendFullOrthographic, ani);
		}

		/// <summary>
		/// Jumps the camera to perspective mode.
		/// </summary>
		public void ForcePerspective()
		{
			_lastSize = TargetPerspectiveSize;
			_lastFieldOfView = fieldOfView;
			_blendValue = BlendFullPerspective;
			_lastBlendValue = BlendFullPerspective;
			_cancelBlend = true;
			UpdateCamera(TargetPerspectiveDistance, distanceValid: true);
		}

		/// <summary>
		/// Jumps the camera to orthographic mode.
		/// </summary>
		public void ForceOrthographic()
		{
			_lastSize = TargetOrthographicSize;
			_lastFieldOfView = TargetOrthographicFieldOfView;
			_blendValue = BlendFullOrthographic;
			_lastBlendValue = BlendFullOrthographic;
			_cancelBlend = true;
			UpdateCamera(TargetOrthographicDistance, distanceValid: false);
		}

		/// <summary>
		/// Jumps the camera to the last perspective step before it would become orthographic.
		/// It's unlikely you want to use this, unless you're just testing the limits of floating point numbers.
		/// </summary>
		public void ForceAlmostOrthographic()
		{
			// force the camera into orthographic mode.
			ForceOrthographic();
			Perspective();
			_cancelBlend = true;
			UpdateProjectionBlend(Mathf.Epsilon, forceValidDistance: true);
		}
	}
}
