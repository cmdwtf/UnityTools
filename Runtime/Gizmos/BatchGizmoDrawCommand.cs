using System;

using cmdwtf.UnityTools.Gizmos;

using UnityEditor;

using UnityEngine;

namespace cmdwtf.UnityTools.Gizmos
{
	internal abstract class BatchGizmoDrawCommand : IBatchGizmoDrawCommand, IComparable,
													IComparable<IBatchGizmoDrawCommand>
	{
		#region Implementation of ISortedGizmoDrawCommand

		/// <inheritdoc />
		public Color Color { get; set; }

		/// <inheritdoc />
		public Vector3 Position { get; set; }

		/// <inheritdoc />
		public bool IsTransparent { get; set; }

		/// <inheritdoc />
		public bool IsWireframe { get; set; }

		/// <summary>
		/// A value representing how far the gizmo is away from the origin.
		/// </summary>
		public float Depth { get; private set; }

		public virtual int SortPass => 0;

		/// <inheritdoc />
		public void UpdateDepth(Vector3 origin) => Depth = Vector3.Distance(origin, Position);

		/// <inheritdoc />
		public void Draw()
		{
			UnityEngine.Gizmos.color = Color;
			Draw(IsWireframe);
		}

		/// <inheritdoc />
		public void DrawText(string text) => Handles.Label(Position, text);

		#endregion

		/// <inheritdoc cref="Draw"/>
		protected abstract void Draw(bool wireFrame);

		#region Implementation of IComparable<in ISortedGizmoDrawCommand>

		/// <inheritdoc />
		public int CompareTo(IBatchGizmoDrawCommand other)
		{
			int sortDirection = IsTransparent || IsWireframe || Color.a < 1f
									? -1
									: 1;

			int v = other.SortPass.CompareTo(SortPass);
			return v == 0 ? other.Depth.CompareTo(Depth) * sortDirection : v;
		}

		#endregion

		#region Implementation of IComparable

		/// <inheritdoc />
		public int CompareTo(object obj) => CompareTo(obj as IBatchGizmoDrawCommand);

		#endregion
	}

	internal class DrawSphereCommand : BatchGizmoDrawCommand
	{
		public float Radius { get; set; }

		protected override void Draw(bool wireFrame)
		{
			if (wireFrame)
			{
				UnityEngine.Gizmos.DrawWireSphere(Position, Radius);
			}
			else
			{
				UnityEngine.Gizmos.DrawSphere(Position, Radius);
			}
		}
	}

	internal class DrawCubeCommand : BatchGizmoDrawCommand
	{
		public Vector3 Size { get; set; }

		protected override void Draw(bool wireFrame)
		{
			if (wireFrame)
			{
				UnityEngine.Gizmos.DrawWireCube(Position, Size);
			}
			else
			{
				UnityEngine.Gizmos.DrawCube(Position, Size);
			}
		}
	}

	internal class DrawMeshCommand : BatchGizmoDrawCommand
	{
		public Vector3 Scale { get; set; }
		public Quaternion Rotation { get; set; }
		public Mesh Mesh { get; set; }

		protected override void Draw(bool wireFrame)
		{
			if (wireFrame)
			{
				UnityEngine.Gizmos.DrawWireMesh(Mesh, Position, Rotation, Scale);
			}
			else
			{
				UnityEngine.Gizmos.DrawMesh(Mesh, Position, Rotation, Scale);
			}
		}
	}

	internal class DrawLabelCommand : BatchGizmoDrawCommand
	{
		public string Text { get; set; }

		#region Overrides of SortedGizmoDrawCommandBase

		/// <inheritdoc />
		public override int SortPass => 1;

		/// <inheritdoc />
		protected override void Draw(bool wireFrame) => DrawText(Text);

		#endregion
	}
}
