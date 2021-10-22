using System.Linq;

using UnityEngine;

namespace cmdwtf.UnityTools
{
	public static class DebugTools
	{
		public static void DrawAxis(Vector3 position = default, float scale = 1.0f)
		{
			Debug.DrawRay(position, Vector3.up * scale,      Color.green);
			Debug.DrawRay(position, Vector3.right * scale,   Color.red);
			Debug.DrawRay(position, Vector3.forward * scale, Color.blue);
		}
		
		public enum LogStackTraceLevel {Log = 0, Warning, Error};
		public static void LogStackTrace(LogStackTraceLevel lvl = LogStackTraceLevel.Warning)
		{
			var stackTrace = new System.Diagnostics.StackTrace(1);
			System.Diagnostics.StackFrame[] stackFrames = stackTrace.GetFrames();

			string msg = "";
			if (stackFrames != null)
			{
				msg = stackFrames.Aggregate(msg, (current, stackFrame) => current + $"{stackFrame.GetMethod().Name}\n");
			}

			switch (lvl)
			{
				case LogStackTraceLevel.Log:
					Debug.Log(msg);
					break;
				case LogStackTraceLevel.Warning:
					Debug.LogWarning(msg);
					break;
				case LogStackTraceLevel.Error:
					Debug.LogError(msg);
					break;
				default:
					Debug.LogError(msg);
					break;
			}
		}
	}
}
