using UnityEngine;
using UnityEngine.InputSystem;

namespace cmdwtf.UnityTools.Input
{
#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoad]
#endif
	public class DebugOnlyProcessor : InputProcessor<float>
	{
		internal bool _allowInDebugBuild = true;
		internal bool _allowInReleaseBuild = false;
		internal bool _allowInEditor = true;

		private bool AlwaysAllow => _allowInEditor && _allowInDebugBuild && _allowInDebugBuild;

#if UNITY_EDITOR
		static DebugOnlyProcessor()
		{
			Initialize();
		}
#endif

		[RuntimeInitializeOnLoadMethod]
		private static void Initialize()
		{
			InputSystem.RegisterProcessor<DebugOnlyProcessor>();
		}

		public override float Process(float value, InputControl control)
		{
			if (AlwaysAllow)
			{
				return value;
			}

			if (Application.isEditor)
			{
				if (!_allowInEditor)
				{
					value = 0;
				}
			}
			else
			{
				if (Debug.isDebugBuild)
				{
					if (!_allowInDebugBuild)
					{
						value = 0;
					}
				}
				else
				{
					if (!_allowInReleaseBuild)
					{
						value = 0;
					}
				}
			}

			return value;
		}
	}
}
