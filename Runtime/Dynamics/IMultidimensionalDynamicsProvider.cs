using System;
using System.Collections.Generic;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// An interface implemented by types that provide a number
	/// of dynamic system dimensions.
	/// </summary>
	public interface IMultidimensionalDynamicsProvider
	{
		int DimensionCount { get; }
		IDynamicsSystem GetDynamicsSystemForDimension(int dimensionIndex);
		IEnumerable<IDynamicsSystem> GetDynamicsEnumerator();
	}
}
