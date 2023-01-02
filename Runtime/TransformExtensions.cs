using System.Linq;

using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class TransformExtensions
	{
		public static int ActiveChildCount(this Transform target)
			=> target.Cast<Transform>().Count(child => child.gameObject.activeSelf);

		public static bool IsChildOf(this Transform target, Transform potentialParent)
		{
			Transform test = target;

			while (test != null)
			{
				if (test == potentialParent)
				{
					return true;
				}

				test = test.parent;
			}

			return false;
		}

		public static bool IsChildOf(this Transform target, GameObject potentialParent)
			=> target.IsChildOf(potentialParent.transform);

		public static bool IsChildOf(this Transform target, Component potentialParent)
			=> target.IsChildOf(potentialParent.transform);

		public static void SetPositionAs(this Transform t, GameObject go)
		{
			Vector3 position = go.transform.position;
			t.position = new Vector3(position.x, position.y, position.z);
		}

		public static void SetPositionAs(this Transform t, Vector3 pos)
		{
			t.position = new Vector3(pos.x, pos.y, pos.z);
		}

		public static void MoveBy(this Transform t, float x, float y, float z)
		{
			Vector3 position = t.position;
			position = new Vector3(position.x + x, position.y + y, position.z + z);
			t.position = position;
		}

		public static void MoveBy(this GameObject go, float x, float y, float z)
		{
			go.transform.MoveBy(x, y, z);
		}

		public static void RotateTo(this Transform t, float x, float y, float z)
		{
			t.eulerAngles = new Vector3(x, y, z);
		}

		public static void ScaleTo(this Transform t, float x, float y, float z)
		{
			t.localScale = new Vector3(x, y, z);
		}

		public static float DistanceTo(this Transform t, Transform other) => t.DistanceTo(other.position);

		public static float DistanceTo(this Transform t, GameObject other) => t.DistanceTo(other.transform.position);

		public static float DistanceTo(this Transform t, Component other) => t.DistanceTo(other.transform.position);

		public static float DistanceTo(this Transform t, Vector3 point) => (t.position - point).magnitude;

		public static float DistanceFrom(this Transform t, Transform other) => t.DistanceFrom(other.position);

		public static float DistanceFrom(this Transform t, GameObject other) => t.DistanceFrom(other.transform.position);

		public static float DistanceFrom(this Transform t, Component other) => t.DistanceFrom(other.transform.position);

		public static float DistanceFrom(this Transform t, Vector3 point) => (point - t.position).magnitude;

		public static float FlatAngleTo(this Transform t, Transform other) => t.FlatAngleTo(other.position);

		public static float FlatAngleTo(this Transform t, GameObject other) => t.FlatAngleTo(other.transform.position);

		public static float FlatAngleTo(this Transform t, Component other) => t.FlatAngleTo(other.transform.position);

		public static float FlatAngleTo(this Transform t, Vector3 other)
			=> Math.FlatAngle(other - t.transform.position);

		public static float FlatAngleFrom(this Transform t, Transform other) => t.FlatAngleFrom(other.position);

		public static float FlatAngleFrom(this Transform t, GameObject other) => t.FlatAngleFrom(other.transform.position);

		public static float FlatAngleFrom(this Transform t, Component other) => t.FlatAngleFrom(other.transform.position);

		public static float FlatAngleFrom(this Transform t, Vector3 other)
			=> Math.FlatAngle(t.transform.position - other);
	}
}
