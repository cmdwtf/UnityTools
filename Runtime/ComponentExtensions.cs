using System.Linq;

using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class ComponentExtensions
	{
		public static bool IsChildOf(this Component testComponent, Transform potentialParent)
			=> testComponent.transform.IsChildOf(potentialParent);

		public static bool IsChildOf(this Component testComponent, GameObject potentialParent)
			=> testComponent.transform.IsChildOf(potentialParent.transform);

		public static bool IsChildOf(this Component testComponent, Component potentialParent)
			=> testComponent.transform.IsChildOf(potentialParent.transform);

		public static float DistanceTo(this Component component, Transform other)
			=> component.transform.DistanceTo(other.position);

		public static float DistanceTo(this Component component, GameObject other)
			=> component.transform.DistanceTo(other.transform.position);

		public static float DistanceTo(this Component component, Component other)
			=> component.transform.DistanceTo(other.transform.position);

		public static float DistanceTo(this Component component, Vector3 point)
			=> component.transform.DistanceTo(point);

		public static float DistanceFrom(this Component component, Transform other)
			=> component.transform.DistanceFrom(other.position);

		public static float DistanceFrom(this Component component, GameObject other)
			=> component.transform.DistanceFrom(other.transform.position);

		public static float DistanceFrom(this Component component, Component other)
			=> component.transform.DistanceFrom(other.transform.position);

		public static float DistanceFrom(this Component component, Vector3 point)
			=> component.transform.DistanceFrom(point);

		public static float FlatAngleTo(this Component component, Transform other)
			=> component.transform.FlatAngleTo(other.position);

		public static float FlatAngleTo(this Component component, GameObject other)
			=> component.transform.FlatAngleTo(other.transform.position);

		public static float FlatAngleTo(this Component component, Component other)
			=> component.transform.FlatAngleTo(other.transform.position);

		public static float FlatAngleTo(this Component component, Vector3 point)
			=> component.transform.FlatAngleTo(point);

		public static float FlatAngleFrom(this Component component, Transform other)
			=> component.transform.FlatAngleFrom(other.position);

		public static float FlatAngleFrom(this Component component, GameObject other)
			=> component.transform.FlatAngleFrom(other.transform.position);

		public static float FlatAngleFrom(this Component component, Component other)
			=> component.transform.FlatAngleFrom(other.transform.position);

		public static float FlatAngleFrom(this Component component, Vector3 point)
			=> component.transform.FlatAngleFrom(point);

		/// <summary>
		/// Functions similarly <see cref="Object.FindObjectOfType{T}()"/>, but returns the
		/// component closest to this game object in 3D space.
		/// </summary>
		/// <param name="component">The current game object.</param>
		/// <typeparam name="T">The type of component to find.</typeparam>
		/// <returns>The closest component of the desired type, or <see langword="null"/> if none is found.</returns>
		public static T FindClosestObjectOfType<T>(this Component component) where T : Component
		{
			if (component == null)
			{
				return null;
			}

			Vector3 thisPos = component.gameObject.transform.position;

			return Object.FindObjectsOfType<T>()
						 .OrderBy(o => (o.gameObject.transform.position - thisPos).sqrMagnitude)
						 .FirstOrDefault();
		}
	}
}
