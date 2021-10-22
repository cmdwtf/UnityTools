using System;

namespace cmdwtf.UnityTools
{
	public static class DateTimeExtensions
	{
		private const string NiceFileStringFormat = "yyyy-MM-dd_HH-mm-ss";
		public static string ToNiceFileString(this DateTime dt) => dt.ToString(NiceFileStringFormat);
		public static string ToNiceFileString(this DateTimeOffset dto) => dto.ToString(NiceFileStringFormat);
	}
}
