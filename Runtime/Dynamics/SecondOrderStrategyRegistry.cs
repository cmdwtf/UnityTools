using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace cmdwtf.UnityTools.Dynamics
{
	/// <summary>
	/// A simple registry that auto-creates second order solving strategies found in the app domain.
	/// </summary>
	internal static class SecondOrderStrategyRegistry
	{
		private static readonly Dictionary<SecondOrderSolvingStrategy, ISecondOrderSolvingStrategyF> Registry;

		static SecondOrderStrategyRegistry()
		{
			// grab all the impls and match them to their enum
			Registry = new Dictionary<SecondOrderSolvingStrategy, ISecondOrderSolvingStrategyF>();

			List<Type> solvers = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.Where(x =>
					typeof(ISecondOrderSolvingStrategyF).IsAssignableFrom(x) &&
					!x.IsInterface &&
					!x.IsAbstract
				).ToList();

			if (solvers.Count <= 0)
			{
				Debug.LogWarning($"Found no {nameof(ISecondOrderSolvingStrategyF)} implementations.");
			}

			const string instanceName = nameof(ISecondOrderSolvingStrategy.Instance);
			const BindingFlags instanceBindingFlags = BindingFlags.Static |
			                                          BindingFlags.Public |
			                                          BindingFlags.GetProperty |
			                                          BindingFlags.FlattenHierarchy;

			foreach (Type solverType in solvers)
			{
				PropertyInfo[] allProps = solverType.GetProperties(instanceBindingFlags);
				PropertyInfo instancePropertyInfo = allProps.FirstOrDefault(p => p.Name == instanceName);

				if (instancePropertyInfo is null)
				{
					Debug.LogWarning($"Failed to find property: {solverType.FullName}.{instanceName}");
					Debug.Log($"{allProps.Length} properties: " + string.Join(", ", allProps.Select(p => p.Name)));
					continue;
				}

				if (instancePropertyInfo.GetValue(null) is ISecondOrderSolvingStrategyF solver)
				{
					Registry.Add(solver.StrategyType, solver);
					continue;
				}

				Debug.LogWarning($"Failed to create instance of {solverType.FullName}.");
			}

		}

		public static ISecondOrderSolvingStrategyF Get(SecondOrderSolvingStrategy strategyType)
		{
			if (Registry == null || Registry.ContainsKey(strategyType) == false)
			{
				return null;
			}

			return Registry[strategyType];
		}
	}
}
