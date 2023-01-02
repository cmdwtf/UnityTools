namespace UnityCoroutineManager
{
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
