// Copyright © 2021 Chris Marc Dailey (nitz) <https://cmd.wtf>
//
// 5 years later, the derivation from this code, done by Wildan Mubarok <http://wellosoft.wordpress.com>
// Everyone is still granted non-exclusive license to do anything at all with this code.
//
// Copyright (c) 2011, Ken Rockot  <k-e-n-@-REMOVE-CAPS-AND-HYPHENS-oz.gs>.  All rights reserved.
// Everyone is granted non-exclusive license to do anything at all with this code.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityCoroutineManager
{

	/// A Task object represents a coroutine.  Tasks can be started, paused, and stopped.
	/// It is an error to attempt to start a task that has been stopped or which has
	/// naturally terminated.
	public class Task : IFlushable
	{
		/// Returns true if and only if the coroutine is running. Paused tasks
		/// are considered to be running.
		public bool Running
		{
			get { return task.Running; }
		}

		/// Returns true if and only if the coroutine is currently paused.
		public bool Paused
		{
			get { return task.Paused; }
		}

		/// Determine whether this task is used once, or false if not
		/// If set to false, then it's your responsibility to call Flush() if this class is no longer use.
		public bool flushAtFinish = true;

		/// Delegate for termination subscribers.  manual is true if and only if
		/// the coroutine was stopped with an explicit call to Stop().
		public delegate void FinishedHandler(bool manual);

		/// Termination event.  Triggered when the coroutine completes execution.
		public event FinishedHandler Finished;


		/// Begins execution of the coroutine
		public void Start()
		{
			/*if(Running)
				task.Restart();
			else*/
			task.Start();
		}

		/// Discontinues execution of the coroutine at its next yield.
		public void Stop()
		{
			task.Stop();
		}

		public void Pause()
		{
			task.Pause();
		}

		public void Unpause()
		{
			task.Unpause();
		}

		public void Reset()
		{
			task.Restart();
		}

		void TaskFinished(bool manual)
		{
			FinishedHandler handler = Finished;
			if (handler != null)
				handler(manual);
			if (flushAtFinish)
				Flush();
		}

		TaskManager.TaskState task;



		/// Creates a new Task object for the given coroutine.
		///
		/// If autoStart is true (default) the task is automatically started
		/// upon construction.

		public Task() { }

		/// Don't use this, use Task.Get instead to be able get from
		/// unused task, Preveting futher GC Allocates
		public Task(IEnumerator c, bool autoStart = true)
		{
			task = TaskManager.CreateTask(c);
			task.Finished += TaskFinished;
			if (autoStart)
				Start();
		}

		/// Initialize new Task, or get from unused stack
		public static Task Get(IEnumerator c, bool autoStart = true)
		{
			Task t = ObjPool<Task>.Get();
			if (t.task == null)
				t.task = TaskManager.CreateTask(c);
			else
				t.task.coroutine = c;
			t.task.Finished += t.TaskFinished;
			if (autoStart)
				t.Start();
			return t;
		}

		/// Delegate variant, for the simplicity of a sake
		/// Using linear interpolation
		public static Task Get(CallBack c, float totalTime, bool autoStart = true)
		{
			return Get(Iterator(c, totalTime));
		}

		/// Delegate variant, for the simplicity of a sake
		/// Customize your own interpolation type
		public static Task Get(CallBack c,
							   float totalTime,
							   InterpolationType interpolType,
							   bool inverted = false,
							   bool autoStart = true
		)
		{
			return Get(Iterator(c, totalTime, interpolType, inverted));
		}

		public delegate void CallBack(float t);

		static IEnumerator Iterator(CallBack call, float totalTim)
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

		static IEnumerator Iterator(CallBack call,
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
					t = 1 - t;
				//The time that returns is always normalized between 0...1
				call(t);
				yield return null;
			}

			//When it's done, make sure we complete the time with perfect 1 (or 0)
			call(inverted ? 0f : 1f);
		}

		//Optimizer stuff ---------
		bool m_flushed;

		public bool GetFlushed()
		{
			return m_flushed;
		}

		public void SetFlushed(bool flushed)
		{
			m_flushed = flushed;
		}

		public void Flush()
		{
			task.Stop();
			task.Finished -= TaskFinished;
			ObjPool<Task>.Release(this);
		}

		//Task with ID functionality, prevent duplicate Coroutines being run

		static Dictionary<string, Task> idStack = new Dictionary<string, Task>();

		/// Delegate variant, for the simplicity of a sake
		/// Using linear interpolation
		/// Use Id for prevent duplicate coroutines
		public static Task Get(IEnumerator c, string id, bool overrideIfExist = true, bool autoStart = true)
		{
			if (idStack.ContainsKey(id))
			{
				Task t = idStack[id];
				if (overrideIfExist)
				{
					if (c == t.task.coroutine)
						t.task.Restart();
					else
					{
						t.Stop();
						t.task.coroutine = c;
					}
				}

				if (autoStart)
					t.Start();
				return t;
			}
			else
			{
				Task t = Get(c, autoStart);
				idStack.Add(id, t);
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
		{
			return Get(Iterator(c, totalTime), id, autoStart);
		}

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
		{
			return Get(Iterator(c, totalTime, interpolType, inverted), id, autoStart);
		}

	}

	class TaskManager : MonoBehaviour
	{
		public class TaskState
		{
			public bool Running
			{
				get { return running; }
			}

			public bool Paused
			{
				get { return paused; }
			}

			public delegate void FinishedHandler(bool manual);

			public event FinishedHandler Finished;

			public IEnumerator coroutine;
			bool running;
			bool paused;
			bool stopped;
			bool restart;

			public TaskState(IEnumerator c)
			{
				coroutine = c;
			}

			public void Pause()
			{
				paused = true;
			}

			public void Unpause()
			{
				paused = false;
			}

			public void Restart()
			{
				restart = true;
			}

			public void Start()
			{
				running = true;
				stopped = false;
				singleton.StartCoroutine(CallWrapper());
			}

			public void Stop()
			{
				stopped = true;
				running = false;
			}

			IEnumerator CallWrapper()
			{
				yield return null;
				IEnumerator e = coroutine;
				while (running)
				{
					if (paused)
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
							running = false;
						}
					}
				}

				FinishedHandler handler = Finished;
				if (handler != null)
					handler(stopped);
			}
		}

		static TaskManager singleton;

		public static TaskState CreateTask(IEnumerator coroutine)
		{
			if (singleton == null)
			{
				GameObject go = new GameObject("TaskManager");
				singleton = go.AddComponent<TaskManager>();
			}

			return new TaskState(coroutine);
		}
	}

	public enum InterpolationType
	{
		/// Standard linear interpolation
		Linear = 0,

		/// Smooth fade interpolation
		SmoothStep = 1,

		/// Smoother fade interpolation than SmoothStep
		SmootherStep = 2,

		/// Sine interpolation, smoothing at the end
		Sinerp = 3,

		/// Cosine interpolation, smoothing at the start
		Coserp = 4,

		/// Extreme bend towards end, low speed at end
		Square = 5,

		/// Extreme bend toward start, high speed at end
		Quadratic = 6,

		/// Stronger bending than Quadratic
		Cubic = 7,

		/// Spherical interpolation, vertical speed at start
		CircularStart = 8,

		/// Spherical interpolation, vertical speed at end
		CircularEnd = 9,

		/// Pure Random interpolation
		Random = 10,

		/// Random interpolation with linear constraining at 0..1
		RandomConstrained = 11
	}
}
