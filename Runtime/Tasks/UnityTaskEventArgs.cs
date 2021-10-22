using System;

namespace cmdwtf.UnityTools.Tasks
{
	public class UnityTaskEventArgs : EventArgs
	{
		public bool IsForced { get; }

		public UnityTaskEventArgs(bool isForced)
		{
			IsForced = isForced;
		}
	}
	
	public class UnityTaskEventArgs<TReturnType> : UnityTaskEventArgs
	{
		public TReturnType ReturnValue { get; }

		public UnityTaskEventArgs(bool isForced, TReturnType returnValue)
			: base(isForced)
		{
			ReturnValue = returnValue;
		}
	}
}
