using UnityEngine;

namespace cmdwtf.UnityTools.Gizmos
{
	internal interface IBatchGizmoDrawCommand
	{
		/// <summary>
		/// The color that should be used to draw the gizmo.
		/// </summary>
		Color Color { get; }

		/// <summary>
		/// Where the gizmo should be drawn.
		/// </summary>
		Vector3 Position { get; }

		/// <summary>
		/// If <see langword="true">, the gizmo should be drawn in wireframe if able.
		/// </summary>
		bool IsWireframe { get; }

		/// <summary>
		/// If <see langword="true">, the gizmo should be drawn when other transparent objects are drawn.
		/// </summary>
		bool IsTransparent { get; }

		/// <summary>
		/// A numerical value representing the distance from the origin to the gizmo.
		/// </summary>
		float Depth { get; }

		/// <summary>
		/// What draw pass the gizmo should be drawn in.
		/// </summary>
		int SortPass { get; }

		/// <summary>
		/// Updates <see cref="Depth"/> based on the new <see cref="origin"/>.
		/// </summary>
		/// <param name="origin">The point to use as the start of depth calculations.</param>
		void UpdateDepth(Vector3 origin);

		/// <summary>
		/// Causes the gizmo to draw now.
		/// </summary>
		void Draw();

		/// <summary>
		/// Causes the gizmo to draw the given string at it's position.
		/// </summary>
		/// <param name="text">The text to draw.</param>
		void DrawText(string text);
	}
}
