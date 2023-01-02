// Copyright © 2021-2022 Chris Marc Dailey (nitz) <https://cmd.wtf>
//
// 5 years later, the derivation from this code, done by Wildan Mubarok <http://wellosoft.wordpress.com>
// Everyone is still granted non-exclusive license to do anything at all with this code.
//
// Copyright (c) 2011, Ken Rockot  <k-e-n-@-REMOVE-CAPS-AND-HYPHENS-oz.gs>.  All rights reserved.
// Everyone is granted non-exclusive license to do anything at all with this code.
using System.Collections;

namespace UnityCoroutineManager
{
	internal class TaskState
	{
		public bool Running { get; private set; }

		public bool Paused { get; private set; }

		public delegate void FinishedHandler(bool manual);

		public event FinishedHandler Finished;

		public IEnumerator coroutine;
		private bool stopped;
		private bool restart;

		public TaskState(IEnumerator c)
		{
			coroutine = c;
		}

		public void Pause() => Paused = true;

		public void Unpause() => Paused = false;

		public void Restart() => restart = true;

		public void Start()
		{
			if (TaskManager.Instance == null)
			{
				return;
			}

			Running = true;
			stopped = false;
			TaskManager.Instance.StartCoroutine(CallWrapper());
		}

		public void Stop()
		{
			stopped = true;
			Running = false;
		}

		private IEnumerator CallWrapper()
		{
			yield return null;
			IEnumerator e = coroutine;
			while (Running)
			{
				if (Paused)
					yield return null;
				else if (restart)
				{
					restart = false;
					if (e != null)
						e.Reset();
				}
				else
				{
					if (e != coroutine)
						e = coroutine;
					if (e != null && e.MoveNext())
					{
						yield return e.Current;
					}
					else
					{
						Running = false;
					}
				}
			}

			FinishedHandler handler = Finished;
			if (handler != null)
				handler(stopped);
		}
	}
}
