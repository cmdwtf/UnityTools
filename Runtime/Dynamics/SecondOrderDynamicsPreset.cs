
namespace cmdwtf.UnityTools.Dynamics
{
	internal readonly struct SecondOrderDynamicsPreset : IDynamicsPreset
	{
		public readonly string Name;
		public readonly ValueRange Frequency;
		public readonly ValueRange Damping;
		public readonly ValueRange Responsiveness;
		public readonly ValueRange DeltaTimeScale;

		public SecondOrderDynamicsPreset(ValueRange frequency,
										 ValueRange damping,
										 ValueRange responsiveness,
										 ValueRange deltaTimeScale
		)
			: this(string.Empty, frequency, damping, responsiveness, deltaTimeScale)
		{

		}

		public SecondOrderDynamicsPreset(string name, ValueRange frequency, ValueRange damping, ValueRange responsiveness, ValueRange? deltaTimeScale = null)
		{
			Name = name;
			Frequency = frequency;
			Damping = damping;
			Responsiveness = responsiveness;
			DeltaTimeScale = deltaTimeScale ?? 1f;
		}

		public SecondOrderDynamicsPreset(float frequency, float damping, float responsiveness, float deltaTimeScale = 1.0f)
			: this(string.Empty, (ValueRange)frequency, damping, responsiveness, deltaTimeScale)
		{

		}
		public SecondOrderDynamicsPreset(string name, float frequency, float damping, float responsiveness, float deltaTimeScale = 1.0f)
			: this(name, (ValueRange)frequency, damping, responsiveness, deltaTimeScale)
		{
		}

		public static SecondOrderDynamicsPreset FromSettings(SecondOrderSettings settings)
			=> new(
			settings.frequency,
			settings.damping,
			settings.responsiveness,
			settings.deltaTimeScale
		);
	}
}
