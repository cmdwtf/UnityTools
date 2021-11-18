namespace cmdwtf.UnityTools
{
	public interface ITimeProvider
	{
		double Period { get; set;  }
		float PeriodF { get; set; }
		double NowSeconds { get; }
		float NowSecondsF { get; }
		double DeltaSeconds { get; }
		float DeltaSecondsF { get; }
		long UnixTimestamp { get; }
		double Update();
		float UpdateF();
	}
}
