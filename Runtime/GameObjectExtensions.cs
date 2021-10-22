using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class GameObjectExtensions
	{
		public static void BroadcastAll(this GameObject _, string fun, object msg)
		{
			var gos = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));
			foreach (GameObject go in gos)
			{
				if (go && go.transform.parent == null)
				{
					go.gameObject.BroadcastMessage(fun, msg, SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		public static bool IsChildOf(this GameObject testObject, Transform potentialParent)
			=> testObject.transform.IsChildOf(potentialParent);

		public static bool IsChildOf(this GameObject testObject, GameObject potentialParent)
			=> testObject.transform.IsChildOf(potentialParent.transform);
		
		public static bool IsChildOf(this GameObject testObject, Component potentialParent)
			=> testObject.transform.IsChildOf(potentialParent.transform);

		public static void StopAllParticleEffects(this GameObject go)
		{
			ParticleSystem ps;
			ps = go.GetComponent<ParticleSystem>();

			if (ps != null)
			{
				ps.Stop();
			}

			ParticleSystem[] psa = go.GetComponentsInChildren<ParticleSystem>();

			foreach (ParticleSystem cps in psa)
			{
				cps.Stop();
			}
		}
		
		public static void SetPositionAs(this GameObject lhs, GameObject rhs)
		{
			lhs.transform.SetPositionAs(rhs);
		}

		public static void SetPositionAs(this GameObject lhs, Vector3 rhs)
		{
			lhs.transform.SetPositionAs(rhs);
		}
		
		public static void RotateTo(this GameObject go, float x, float y, float z)
		{
			go.transform.RotateTo(x, y, z);
		}
		
		public static void ScaleTo(this GameObject go, float x, float y, float z)
		{
			go.transform.ScaleTo(x, y, z);
		}

		public static float DistanceTo(this GameObject go, Transform other)
			=> go.transform.DistanceTo(other.position);

		public static float DistanceTo(this GameObject go, GameObject other)
			=> go.transform.DistanceTo(other.transform.position);
		
		public static float DistanceTo(this GameObject go, Component other)
			=> go.transform.DistanceTo(other.transform.position);
		
		public static float DistanceTo(this GameObject go, Vector3 point)
			=> go.transform.DistanceTo(point);
		
		#region Cube Bounds of Object
		
		public static Vector3 GetLeft(this GameObject go)
			=> (go.transform.position - go.transform.right * go.transform.localScale.x/2);

		public static Vector3 GetRight(this GameObject go)
			=> (go.transform.position + go.transform.right * go.transform.localScale.x/2);

		public static Vector3 GetTop(this GameObject go)
			=> (go.transform.position + go.transform.up * go.transform.localScale.y/2);

		public static Vector3 GetBottom(this GameObject go)
			=> (go.transform.position - go.transform.up * go.transform.localScale.y/2);

		public static Vector3 GetFront(this GameObject go)
			=> (go.transform.position + go.transform.forward * go.transform.localScale.z/2);

		public static Vector3 GetBack(this GameObject go)
			=> (go.transform.position - go.transform.forward * go.transform.localScale.z/2);

		#endregion Cube Bounds of Object
	}
}
