namespace cmdwtf.UnityTools.Filters
{
	public interface IBiQuadFilter<T> : IFilter<T>
	{
		/// <summary>
		/// Frequency to filter at.
		/// </summary>
		float Frequency { get; }

		/// <summary>
		/// The rate the value is sampled at.
		/// </summary>
		int SampleRate { get; }

		/// <summary>
		/// Resonance amount, from sqrt(2) to ~ 0.1
		/// </summary>
		float Resonance { get; }
	}
}
