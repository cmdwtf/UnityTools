namespace cmdwtf.UnityTools.Dynamics
{
	public interface ISimulatableDynamicsSystem : IDynamicsSystem
	{
#if UNITY_EDITOR
		/// <summary>
		/// Causes the system to store a backup of its state so that it can be
		/// used with a different state for a simulation.
		/// </summary>
		void PushState();

		/// <summary>
		/// Restores the original state that was earlier pushed.
		/// </summary>
		/// <returns>True, if the state was restored.</returns>
		bool PopState();

		/// <summary>
		/// Applies settings from the given preset value.
		/// </summary>
		/// <param name="preset">The preset settings to apply.</param>
		void SetTemporarySettings(IDynamicsPreset preset);

		/// <summary>
		/// Clears any set temporary settings.
		/// </summary>
		void ClearTemporarySettings();

		/// <summary>
		/// Resets the system with a new IV, but doesn't store it as original iv.
		/// </summary>
		/// <param name="iv">The temporary initialization value.</param>
		void ResetTemporarySim(float iv);

		/// <summary>
		/// Updates the system with a given delta time and value.
		/// </summary>
		/// <param name="deltaTime">The time since the previous update.</param>
		/// <param name="newValue">The new input for the system.</param>
		/// <returns>The updated value.</returns>
		float UpdateSim(float deltaTime, float newValue);

		/// <summary>
		/// Returns true if the system's current state is stable.
		/// </summary>
		bool IsStable { get; }

		/// <summary>
		/// Gets a value representing the current stability of the system.
		/// </summary>
		public StabilityState StabilityState { get; }
#endif // UNITY_EDITOR
	}
}
