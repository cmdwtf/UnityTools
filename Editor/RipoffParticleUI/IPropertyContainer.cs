namespace cmdwtf.UnityTools.Editor.RipoffParticleUI
{
	/// <summary>
	/// An interface representing a type that contains properties.
	/// </summary>
	public interface IPropertyContainer
	{
		/// <summary>
		/// Will be called when the type should draw any properties it contains.
		/// </summary>
		/// <param name="context">The drawing context.</param>
		void OnDrawProperties(Context context);
	}
}
