using System;
using System.Runtime.InteropServices;

namespace RndWallpaper
{
	public static class WindowsMethods
	{
		[DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SystemParametersInfo(UAction uiAction, uint uiParam, String pvParam, SPIF fWinIni);

		[DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int GetSystemMetrics(int nIndex);

		[DllImport("dwmapi", PreserveSig = false)]
		public static extern void DwmGetColorizationColor(out uint ColorizationColor, [MarshalAs(UnmanagedType.Bool)]out bool ColorizationOpaqueBlend);

		[DllImport("uxtheme", PreserveSig = false)]
		public static extern void GetUserColorPreference(out ImmersiveColorPreference preference, [MarshalAs(UnmanagedType.Bool)] bool forceReload);

		[DllImport("uxtheme", PreserveSig = false)]
		public static extern void SetUserColorPreference(ref ImmersiveColorPreference preference, [MarshalAs(UnmanagedType.Bool)] bool forceCommit);

		[DllImport("user32", CharSet = CharSet.Auto)]
		public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

	}
}