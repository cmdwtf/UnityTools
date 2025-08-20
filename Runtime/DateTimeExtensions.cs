using System;

namespace cmdwtf.UnityTools
{
	public static class DateTimeExtensions
	{
		private const string NiceFileStringFormat = "yyyy-MM-dd_HH-mm-ss";
		private const string NiceFileStringDateOnlyFormat = "yyyy-MM-dd";
		public static string ToNiceFileString(this DateTime dt) => dt.ToString(NiceFileStringFormat);
		public static string ToNiceFileString(this DateTimeOffset dto) => dto.ToString(NiceFileStringFormat);
		public static string ToNiceFileStringDateOnly(this DateTime dt) => dt.ToString(NiceFileStringDateOnlyFormat);
		public static string ToNiceFileStringDateOnly(this DateTimeOffset dto) => dto.ToString(NiceFileStringDateOnlyFormat);
	}
}
