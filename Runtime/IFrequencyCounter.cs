namespace cmdwtf.UnityTools
{
	public interface IFrequencyCounter
	{
		int Samples { get; }
		float Frequency { get; }
		float Period { get; }
		float MeasuredSeconds { get; }
		void Reset();
		void Tick();
		void Update();
		void TickAndUpdate();
	}
}
