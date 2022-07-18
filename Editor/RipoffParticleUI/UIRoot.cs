using UnityEditor;

namespace cmdwtf.UnityTools.Editor.RipoffParticleUI
{
	/// <summary>
	/// An element that represents the root of a <see cref="RipoffParticleUI"/> UI tree.
	/// </summary>
	public class UIRoot : Element
	{
		/// <inheritdoc />
		public override bool IsExpanded => true;

		/// <inheritdoc />
		public override bool Visible => true;

		private Context _context = null;

		/// <summary>
		/// Draws the UI.
		/// </summary>
		/// <param name="serialized">The serialized object the UI should be drawn for.</param>
		/// <returns><see langword="true" /> if the UI detected a change, otherwise <see langword="false"/>.</returns>
		public bool Draw(SerializedObject serialized)
		{
			EditorGUI.BeginChangeCheck();
			using EditorGUI.IndentLevelScope indentReset = new(-EditorGUI.indentLevel - 1);
			_context ??= new Context();
			_context.SerializedObject = serialized;
			OnBeforeDraw(_context);
			OnDraw(_context);
			OnAfterDraw(_context);
			return EditorGUI.EndChangeCheck();
		}
	}
}
