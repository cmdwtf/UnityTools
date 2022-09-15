using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

// concept from https://gist.github.com/bugshake/9adbb023494b7b886c78d1f82e67df65

namespace cmdwtf.UnityTools.Gizmos
{
	/// <summary>
	/// A small batching gizmo drawer that will draw several gizmos in sorted groups.
	/// </summary>
	public static class Sorted
	{
		private const int BatchCommandCapacity = 1000;
		private static List<IBatchGizmoDrawCommand> _batchedCommands;

		private static List<IBatchGizmoDrawCommand> BatchedCommands
		{
			get
			{
				if (_batchedCommands != null)
				{
					return _batchedCommands;
				}

				BatchBegin();
				return _batchedCommands;
			}
		}

		/// <summary>
		/// An alias for <see cref="UnityEngine.Gizmos.color"/>.
		/// </summary>
		// ReSharper disable once InconsistentNaming
		public static Color color
		{
			get
			{
				return
#if UNITY_EDITOR
				UnityEngine.Gizmos.color;
#else
				Color.clear;
#endif // UNITY_EDITOR
			}
			set
			{
#if UNITY_EDITOR
				UnityEngine.Gizmos.color = value;
#endif // UNITY_EDITOR
			}
		}

		/// <summary>
		/// Starts buffering a new batch of Gizmo drawing commands, abandoning any pending gizmo draw commands.
		/// </summary>
		public static void BatchBegin()
		{
#if UNITY_EDITOR
			_batchedCommands = new List<IBatchGizmoDrawCommand>(BatchCommandCapacity);
#endif // UNITY_EDITOR
		}

		private static Camera Camera
		{
			get
			{
#if UNITY_EDITOR
				SceneView sv = SceneView.currentDrawingSceneView;
				if (sv != null && sv.camera != null)
				{
					return sv.camera;
				}
#endif // UNITY_EDITOR
				return Camera.main;
			}
		}

		//private static Matrix4x4 CameraMatrix => Camera.worldToCameraMatrix;

		/// <summary>
		/// Finishes a batch of gizmo draw commands. Sorts and draws them.
		/// </summary>
		public static void BatchEnd()
		{
#if UNITY_EDITOR

			//Matrix4x4 currentCameraMatrix = CameraMatrix;
			Vector3 where = Camera.transform.position;

			foreach (IBatchGizmoDrawCommand cmd in _batchedCommands)
			{
				cmd.UpdateDepth(where);
			}

			// sort by comparator
			IBatchGizmoDrawCommand[] sorted = _batchedCommands.ToArray();
			Array.Sort(sorted);

			// draw
			foreach (IBatchGizmoDrawCommand cmd in sorted)
			{
				cmd.Draw();
			}

			// debug text
			//int scan = 0;
			//foreach (ISortedGizmoDrawCommand cmd in sorted)
			//{
			//	cmd.DrawText($"{cmd.SortValue.ToString2Points()}\n{scan++}");
			//}
#endif
			_batchedCommands.Clear();
		}

		/// <inheritdoc cref="UnityEngine.Gizmos.DrawSphere"/>
		public static void DrawSphere(Vector3 center, float radius)
			=> BatchedCommands.Add(new DrawSphereCommand { Color = color, Position = center, Radius = radius });

		/// <inheritdoc cref="UnityEngine.Gizmos.DrawWireSphere"/>
		public static void DrawWireSphere(Vector3 center, float radius)
			=> BatchedCommands.Add(new DrawSphereCommand { Color = color, Position = center, Radius = radius, IsWireframe = true });

		/// <inheritdoc cref="UnityEngine.Gizmos.DrawCube"/>
		public static void DrawCube(Vector3 center, Vector3 size)
			=> BatchedCommands.Add(new DrawCubeCommand { Color = color, Position = center, Size = size });

		/// <inheritdoc cref="UnityEngine.Gizmos.DrawWireCube"/>
		public static void DrawWireCube(Vector3 center, Vector3 size)
			=> BatchedCommands.Add(new DrawCubeCommand { Color = color, Position = center, Size = size, IsWireframe = true });

		/// <inheritdoc cref="UnityEngine.Gizmos.DrawMesh(UnityEngine.Mesh,int,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3)"/>
		public static void DrawMesh(Mesh mesh, Vector3 center, Quaternion? rotation = null, Vector3? scale = null)
			=> BatchedCommands.Add(new DrawMeshCommand
			{
				Color = color,
				Position = center,
				Mesh = mesh,
				Scale = scale ?? Vector3.one,
				Rotation = rotation ?? Quaternion.identity,
			});

		/// <inheritdoc cref="UnityEngine.Gizmos.DrawWireMesh(UnityEngine.Mesh,int,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Vector3)"/>
		public static void DrawWireMesh(Mesh mesh, Vector3 center, Quaternion? rotation = null, Vector3? scale = null)
			=> BatchedCommands.Add(new DrawMeshCommand
			{
				Color = color,
				Position = center,
				Mesh = mesh,
				Scale = scale ?? Vector3.one,
				Rotation = rotation ?? Quaternion.identity,
				IsWireframe = true,
			});

		public static void Label(Vector3 position, string text)
			=> BatchedCommands.Add(new DrawLabelCommand { Color = color, Position = position, Text = text, });

	}
}
