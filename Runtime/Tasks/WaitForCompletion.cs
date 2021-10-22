using UnityEngine;

namespace cmdwtf.UnityTools.Tasks
{
	public class WaitForCompletion : CustomYieldInstruction
	{
		private UnityTask Task { get; }

		public override bool keepWaiting => Task.IsRunning;

		public WaitForCompletion(UnityTask task)
		{
			Task = task;
		}
	}
}
