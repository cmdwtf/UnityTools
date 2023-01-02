// Copyright © 2021-2022 Chris Marc Dailey (nitz) <https://cmd.wtf>
//
// 5 years later, the derivation from this code, done by Wildan Mubarok <http://wellosoft.wordpress.com>
// Everyone is still granted non-exclusive license to do anything at all with this code.
//
// Copyright (c) 2011, Ken Rockot  <k-e-n-@-REMOVE-CAPS-AND-HYPHENS-oz.gs>.  All rights reserved.
// Everyone is granted non-exclusive license to do anything at all with this code.
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace UnityCoroutineManager
{
	/// A Task object represents a coroutine.  Tasks can be started, paused, and stopped.
	/// It is an error to attempt to start a task that has been stopped or which has
	/// naturally terminated.
	public class Task : IFlushable
	{
		/// Returns true if and only if the coroutine is running. Paused tasks
		/// are considered to be running.
		public bool Running => _state.Running;

		/// Returns true if and only if the coroutine is currently paused.
		public bool Paused => _state.Paused;

		/// Determine whether this task is used once, or false if not
		/// If set to false, then it's your responsibility to call Flush() if this class is no longer use.
		public bool FlushAtFinish { get; set; } = true;

		/// Delegate for termination subscribers.  manual is true if and only if
		/// the coroutine was stopped with an explicit call to Stop().
		public delegate void FinishedHandler(bool manual);

		/// Termination event.  Triggered when the coroutine completes execution.
		public event FinishedHandler Finished;

		/// The current task state.
		private TaskState _state;

		/// Begins execution of the coroutine
		public void Start() =>
			/*if(Running)
				state.Restart();
			else*/
			_state.Start();

		/// Discontinues execution of the coroutine at its next yield.
		public void Stop() => _state.Stop();

		public void Pause() => _state.Pause();

		public void Unpause() => _state.Unpause();

		public void Reset() => _state.Restart();

		private void TaskFinished(bool manual)
		{
			FinishedHandler handler = Finished;
			if (handler != null)
			{
				handler(manual);
			}

			if (FlushAtFinish)
			{
				Flush();
			}
		}

		/// Creates a new Task object for the given coroutine.
		///
		/// If autoStart is true (default) the task is automatically started
		/// upon construction.

		public Task() { }

		/// Don't use this, use Task.Get instead to be able get from
		/// unused task, Preveting futher GC Allocates
		public Task(IEnumerator c, bool autoStart = true)
		{
			_state = TaskManager.CreateTask(c);
			_state.Finished += TaskFinished;
			if (autoStart)
			{
				Start();
			}
		}

		/// Initialize new Task, or get from unused stack
		public static Task Get(IEnumerator c, bool autoStart = true)
		{
			Task t = ObjPool<Task>.Get();
			if (t._state == null)
			{
				t._state = TaskManager.CreateTask(c);
			}
			else
			{
				t._state.coroutine = c;
			}

			t._state.Finished += t.TaskFinished;
			if (autoStart)
			{
				t.Start();
			}

			return t;
		}

		/// Delegate variant, for the simplicity of a sake
		/// Using linear interpolation
		public static Task Get(CallBack c, float totalTime, bool autoStart = true) => Get(Iterator(c, totalTime));

		/// Delegate variant, for the simplicity of a sake
		/// Customize your own interpolation type
		public static Task Get(CallBack c,
							   float totalTime,
							   InterpolationType interpolType,
							   bool inverted = false,
							   bool autoStart = true
		)
			=> Get(Iterator(c, totalTime, interpolType, inverted));

		public delegate void CallBack(float t);

		private static IEnumerator Iterator(CallBack call, float totalTim)
		{
			float tim = Time.time + totalTim;
			while (tim > Time.time)
			{
				//The time that returns is always normalized between 0...1
				call(1 - ((tim - Time.time) / totalTim));
				yield return null;
			}

			//When it's done, make sure we complete the time with perfect 1
			call(1f);
		}

		private static IEnumerator Iterator(CallBack call,
											float totalTime,
											InterpolationType interpolType,
											bool inverted = false
		)
		{
			float tim = Time.time + totalTime;
			while (tim > Time.time)
			{
				float t = 1 - ((tim - Time.time) / totalTime);
				switch (interpolType)
				{
					case InterpolationType.Linear: break;
					case InterpolationType.SmoothStep:
						t = t * t * (3f - 2f * t);
						break;
					case InterpolationType.SmootherStep:
						t = t * t * t * (t * (6f * t - 15f) + 10f);
						break;
					case InterpolationType.Sinerp:
						t = Mathf.Sin(t * Mathf.PI / 2f);
						break;
					case InterpolationType.Coserp:
						t = 1 - Mathf.Cos(t * Mathf.PI / 2f);
						break;
					case InterpolationType.Square:
						t = Mathf.Sqrt(t);
						break;
					case InterpolationType.Quadratic:
						t = t * t;
						break;
					case InterpolationType.Cubic:
						t = t * t * t;
						break;
					case InterpolationType.CircularStart:
						t = Mathf.Sqrt(2 * t + t * t);
						break;
					case InterpolationType.CircularEnd:
						t = 1 - Mathf.Sqrt(1 - t * t);
						break;
					case InterpolationType.Random:
						t = Random.value;
						break;
					case InterpolationType.RandomConstrained:
						t = Mathf.Max(Random.value, t);
						break;
				}

				if (inverted)
				{
					t = 1 - t;
				}

				//The time that returns is always normalized between 0...1
				call(t);
				yield return null;
			}

			//When it's done, make sure we complete the time with perfect 1 (or 0)
			call(inverted ? 0f : 1f);
		}

		//Optimizer stuff ---------
		private bool _mFlushed;

		public bool GetFlushed() => _mFlushed;

		public void SetFlushed(bool flushed) => _mFlushed = flushed;

		public void Flush()
		{
			_state.Stop();
			_state.Finished -= TaskFinished;
			ObjPool<Task>.Release(this);
		}

		//Task with ID functionality, prevent duplicate Coroutines being run

		private static readonly Dictionary<string, Task> IDStack = new Dictionary<string, Task>();

		/// Delegate variant, for the simplicity of a sake
		/// Using linear interpolation
		/// Use Id for prevent duplicate coroutines
		private static Task Get(IEnumerator c, string id, bool overrideIfExist = true, bool autoStart = true)
		{
			if (IDStack.ContainsKey(id))
			{
				Task t = IDStack[id];
				if (overrideIfExist)
				{
					if (c == t._state.coroutine)
					{
						t._state.Restart();
					}
					else
					{
						t.Stop();
						t._state.coroutine = c;
					}
				}

				if (autoStart)
				{
					t.Start();
				}

				return t;
			}
			else
			{
				Task t = Get(c, autoStart);
				IDStack.Add(id, t);
				return t;
			}
		}

		/// Delegate variant, for the simplicity of a sake
		/// Using linear interpolation
		/// Use Id for prevent duplicate coroutines
		public static Task Get(CallBack c,
							   float totalTime,
							   string id,
							   bool overrideIfExist = true,
							   bool autoStart = true
		)
			=> Get(Iterator(c, totalTime), id, autoStart);

		/// Delegate variant, for the simplicity of a sake
		/// Customize your own interpolation type
		/// Use Id for prevent duplicate coroutines
		public static Task Get(CallBack c,
							   float totalTime,
							   string id,
							   InterpolationType interpolType,
							   bool inverted = false,
							   bool overrideIfExist = true,
							   bool autoStart = true
		)
			=> Get(Iterator(c, totalTime, interpolType, inverted), id, autoStart);
	}
}
