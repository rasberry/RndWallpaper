using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RndWallpaper
{
	public static class WinMethods
	{
		[DllImport("user32", CharSet = CharSet.Auto)]
		public static extern bool GetMonitorInfo(HandleRef hMonitor, ref MonitorInfoEx lpmi);

		[DllImport("user32", ExactSpelling = true)]
		public static extern int GetSystemMetrics(int nIndex);

		[DllImport("gdi32", CharSet = CharSet.Auto)]
		public static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);

		[DllImport("gdi32", CharSet = CharSet.Auto)]
		public static extern int GetDeviceCaps(HandleRef hdc, [MarshalAs(UnmanagedType.I4)] DeviceCap nIndex);

		[DllImport("gdi32")]
		public static extern bool DeleteDC([In] HandleRef hdc);

		[DllImport("user32")]
		public static extern bool EnumDisplayMonitors(HandleRef hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);
		public delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

		[DllImport("ole32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern int CoInitializeEx([In, Optional] IntPtr pvReserved, [In] COINIT dwCoInit);

		[DllImport("ole32")]
		public static extern int CoCreateInstance(
			[In] ref Guid rclsid,
			IntPtr pUnkOuter,
			[MarshalAs(UnmanagedType.I4)] CLSCTX dwClsContext,
			[In] ref Guid riid,
			[Out] out IntPtr ppv);

		[DllImport("kernel32", SetLastError = true)]
		public static extern uint FormatMessage(
			[MarshalAs(UnmanagedType.U4)] Format_Message_Flags dwFlags,
			IntPtr lpSource,
			uint dwMessageId,
			uint dwLanguageId,
			[Out] StringBuilder lpBuffer,
			uint nSize,
			string[] Arguments
		);

	}
}