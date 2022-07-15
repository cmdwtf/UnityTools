using UnityEditor;

namespace cmdwtf.UnityTools.Editor.RipoffParticleUI
{
	public class UIRoot : Element
	{
		/// <inheritdoc />
		public override bool IsExpanded => true;

		public override bool Visible => true;

		private Context _context = null;

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
