using cmdwtf.UnityTools.Dynamics;

namespace cmdwtf.UnityTools.Editor.Dynamics
{
	internal readonly struct DynamicsSimulationConfig
	{
		public ISimulatableDynamicsSystem Dynamics { get; }
		public int SampleCount { get; }
		public float OriginSeconds { get; }
		public float OriginValue { get; }
		public float TargetValue { get; }
		public float DurationSeconds { get; }
		public float StepDeltaTime => DurationSeconds / SampleCount;

		public SecondOrderDynamicsPreset? OverridePreset { get; }

		private const float DefaultSimulationOriginValue = 0.0f;
		private const float DefaultSimulationOriginTime = 0.0f;

		private const float DefaultSimulationTarget = 1f;
		private const float DefaultSimulationSeconds = 1f;

		private const int DefaultSampleCount = 100;

		public DynamicsSimulationConfig(ISimulatableDynamicsSystem dynamics,
										float originTimeSeconds = DefaultSimulationOriginTime,
										float originValue = DefaultSimulationOriginValue,
										float durationSecondsSeconds = DefaultSimulationSeconds,
										float targetValue = DefaultSimulationTarget,
										int sampleCount = DefaultSampleCount,
										SecondOrderDynamicsPreset? overridePreset = null)
		{
			Dynamics = dynamics;
			OriginSeconds = originTimeSeconds;
			OriginValue = originValue;
			SampleCount = sampleCount;
			TargetValue = targetValue;
			DurationSeconds = durationSecondsSeconds;
			OverridePreset = overridePreset;
		}
	}
}
