using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	public enum SecondOrderSolvingStrategy
	{
		[InspectorName("")] // don't allow the user to select unknown in the editor.
		Unknown = -1,
		[Tooltip("No strategy, will always return zero.")]
		None = 0,
		[Tooltip("Linear strategy, modeling y=x.")]
		Linear,
		[Tooltip("Solve the second order system using the Semi-implicit Euler method.")]
		SemiImplicitEulerUnstable,
		[Tooltip("Solve the second order system using the Semi-implicit Euler method, with a forced minimum time step " +
		         "to prevent instabilities.")]
		SemiImplicitEulerStableForcedIterations,
		[Tooltip("Solve the second order system using the Semi-implicit Euler method, with a constrained k2 constant " +
		         "to prevent instabilities.")]
		SemiImplicitEulerStableClampedK2,
		[Tooltip("Solve the second order system using the Semi-implicit Euler method, with a constrained k2 constant, " +
		         "that takes into account negative eigenvalues to prevent instabilities and jitter.")]
		SemiImplicitEulerStableClampedK2NoJitter,
		[Tooltip("Solve the second order system using the pole-zero matching (a.k.a. matched Z-transform method). " +
		         "This will be more accurate than the Euler methods (especially for high velocities), " +
		         "but be more computationally expensive.")]
		PoleZeroMatching,
	}
}
