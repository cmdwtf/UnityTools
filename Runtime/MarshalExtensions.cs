namespace cmdwtf.UnityTools
{
	public static class MarshalExtensions
	{
		public static string GetAnsiString(this System.IntPtr ptr)
			=> System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ptr);

		public static string GetAutoString(this System.IntPtr ptr)
			=> System.Runtime.InteropServices.Marshal.PtrToStringAuto(ptr);
	}
}
