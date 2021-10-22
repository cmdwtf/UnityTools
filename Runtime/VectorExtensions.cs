using System.ComponentModel;

using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class VectorExtensions
	{
		public static float GetAxis(this Vector4 target, Axis axis) 
			=> axis switch
				{
					Axis.X => target.x,
					Axis.Y => target.y,
					Axis.Z => target.z,
					Axis.W => target.w,
					_      => throw new InvalidEnumArgumentException(nameof(axis)),
				};

		public static float GetAxis(this Vector4 target, int axis)
			=> target.GetAxis((Axis)axis);

		public static float GetAxis(this Vector3 target, Axis axis)
			=> axis switch
				{
					Axis.X => target.x,
					Axis.Y => target.y,
					Axis.Z => target.z,
					_      => throw new InvalidEnumArgumentException(nameof(axis)),
				};

		public static float GetAxis(this Vector3 target, int axis) 
			=> target.GetAxis((Axis)axis);

		public static float GetAxis(this Vector2 target, Axis axis)
			=> axis switch 
			{
				Axis.X => target.x,
				Axis.Y => target.y,
				_      => throw new InvalidEnumArgumentException(nameof(axis)),
			};

		public static float GetAxis(this Vector2 target, int axis)
			=> target.GetAxis((Axis)axis);
		
		public static string ToStringData(this Vector2 lhs)
			=> $"X: {lhs.x} Y: {lhs.y}";

		public static string ToStringData(this Vector3 lhs)
			=> $"X: {lhs.x} Y: {lhs.y} Z: {lhs.z}";

		public static string ToStringData(this Vector4 lhs)
			=> $"X: {lhs.x} Y: {lhs.y} Z: {lhs.z} W: {lhs.w}";

		public static Vector2 FlipFlop(this Vector2 lhs)
			=> new Vector2(lhs.y, lhs.x);

		public static bool IsReallyCloseTo(this Vector3 lhs, Vector3 rhs, float epsilon)
		{
			if (Mathf.Abs(lhs.x) > Mathf.Abs(rhs.x) + epsilon) return false;
			if (Mathf.Abs(lhs.x) < Mathf.Abs(rhs.x) - epsilon) return false;
			if (Mathf.Abs(lhs.y) > Mathf.Abs(rhs.y) + epsilon) return false;
			if (Mathf.Abs(lhs.y) < Mathf.Abs(rhs.y) - epsilon) return false;
			if (Mathf.Abs(lhs.z) > Mathf.Abs(rhs.z) + epsilon) return false;
			if (Mathf.Abs(lhs.z) < Mathf.Abs(rhs.z) - epsilon) return false;
			return true;
		}
	}
}
