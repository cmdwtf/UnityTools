// Copyright © 2021-2022 Chris Marc Dailey (nitz) <https://cmd.wtf>
//
// 5 years later, the derivation from this code, done by Wildan Mubarok <http://wellosoft.wordpress.com>
// Everyone is still granted non-exclusive license to do anything at all with this code.
//
// Copyright (c) 2011, Ken Rockot  <k-e-n-@-REMOVE-CAPS-AND-HYPHENS-oz.gs>.  All rights reserved.
// Everyone is granted non-exclusive license to do anything at all with this code.

using UnityEngine;
using System.Collections;

namespace UnityCoroutineManager
{
	internal class TaskManager : MonoBehaviour
	{
		internal static TaskManager Instance { get; private set; }

		public static TaskState CreateTask(IEnumerator coroutine)
		{
			if (Instance == null)
			{
				var go = new GameObject("TaskManager");
				Instance = go.AddComponent<TaskManager>();
			}

			return new TaskState(coroutine);
		}
	}
}
