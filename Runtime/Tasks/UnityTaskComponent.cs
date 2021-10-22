using UnityEngine;

namespace cmdwtf.UnityTools.Tasks
{
	public class UnityTaskComponent : MonoBehaviour
	{
		private static UnityTaskComponent _instance;

		public static UnityTaskComponent Instance
		{
			get
			{
				if (_instance)
				{
					return _instance;
				}

				var unityTaskObject = new GameObject("routine");
				var unityTaskComponent = unityTaskObject.AddComponent<UnityTaskComponent>();

				DontDestroyOnLoad(unityTaskObject);
				_instance = unityTaskComponent;

				return _instance;
			}
		}

		private void Awake()
		{
			if (_instance != null)
			{
				Debug.LogError($"More than one {nameof(UnityTaskComponent)} was attempted to be created.\n" +
							   $"Please use {nameof(UnityTaskComponent)}.{nameof(Instance)} instead.");
			}
		}
	}
}
