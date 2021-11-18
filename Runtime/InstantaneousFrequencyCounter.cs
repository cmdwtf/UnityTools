namespace cmdwtf.UnityTools
{
	public class InstantaneousFrequencyCounter : FrequencyCounterBase
	{
		private float _period;
		public float Period
		{
			get => _period;
			set
			{
				_period = value;
				Reset();
			}
		}

		public int Samples { get; private set; }
		public float Frequency { get; private set; }
		public float MeasuredSeconds { get; private set; }

		private float _elapsed;
		private float _nextUpdate;
		private float _lastUpdate;

		public InstantaneousFrequencyCounter(ITimeProvider timeProvider)
			: base(timeProvider) { }

		public void AddTicks(int sampleCount) => Samples += sampleCount;

		public override void Reset()
		{
			Samples = 0;
			Frequency = 0;
			_elapsed = 0;
			_nextUpdate = Period;
			_lastUpdate = 0;
		}

		public override void Tick() => AddTicks(1);

		public override void Update() => Update(Time.UpdateF());

		public void Update(float deltaSeconds)
		{
			_elapsed += deltaSeconds;

			if (_elapsed < _nextUpdate)
			{
				return;
			}

			MeasuredSeconds = _elapsed - _lastUpdate;
			Frequency = Samples / MeasuredSeconds;
			_nextUpdate = _elapsed + Period;
			_lastUpdate = _elapsed;
			Samples = 0;
		}

		public override string ToString() => $"{Frequency.ToString2Points()} Hz";
	}
}
