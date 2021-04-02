using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RndWallpaper
{
	public static class WinMethods
	{
		[DllImport("user32", CharSet = CharSet.Auto)]
		public static extern bool GetMonitorInfo(
			HandleRef hMonitor,
			ref MonitorInfoEx lpmi
		);

		[DllImport("user32", ExactSpelling = true)]
		public static extern int GetSystemMetrics(int nIndex);

		[DllImport("gdi32", CharSet = CharSet.Auto)]
		public static extern IntPtr CreateDC(
			string lpszDriver,
			string lpszDevice,
			string lpszOutput,
			IntPtr lpInitData
		);

		[DllImport("gdi32", CharSet = CharSet.Auto)]
		public static extern int GetDeviceCaps(
			HandleRef hdc,
			[MarshalAs(UnmanagedType.I4)] DeviceCap nIndex
		);

		[DllImport("gdi32")]
		public static extern bool DeleteDC([In] HandleRef hdc);

		public delegate bool EnumMonitorsDelegate(
			IntPtr hMonitor,
			IntPtr hdcMonitor,
			ref RECT lprcMonitor,
			IntPtr dwData
		);
		[DllImport("user32")]
		public static extern bool EnumDisplayMonitors(
			HandleRef hdc,
			IntPtr lprcClip,
			EnumMonitorsDelegate lpfnEnum,
			IntPtr dwData
		);

		[DllImport("ole32", CharSet = CharSet.Auto, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
		public static extern uint CoInitializeEx(
			[In, Optional] IntPtr pvReserved,
			[In] COINIT dwCoInit
		);

		[DllImport("ole32")]
		public static extern uint CoCreateInstance(
			[In] ref Guid rclsid,
			IntPtr pUnkOuter,
			[MarshalAs(UnmanagedType.I4)] CLSCTX dwClsContext,
			[In] ref Guid riid,
			[Out] out IntPtr ppv);

		[DllImport("kernel32", SetLastError = true)]
		public static extern uint FormatMessage(
			[MarshalAs(UnmanagedType.U4)] FormatMessageFlags dwFlags,
			IntPtr lpSource,
			uint dwMessageId,
			uint dwLanguageId,
			[Out] StringBuilder lpBuffer,
			uint nSize,
			string[] Arguments
		);

		[DllImport("dxva2", ExactSpelling = true, SetLastError = true, PreserveSig = false)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetMonitorTechnologyType(
			[In] HandleRef hMonitor,
			[Out] out DisplayTechnologyType pdtyDisplayTechnologyType
		);

		[DllImport("dxva2", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetPhysicalMonitorsFromHMONITOR(
			HandleRef hMonitor,
			uint dwPhysicalMonitorArraySize,
			PHYSICAL_MONITOR[] pPhysicalMonitorArray
		);

		[DllImport("dxva2", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DestroyPhysicalMonitors(
			[In] uint dwPhysicalMonitorArraySize,
			PHYSICAL_MONITOR[] pPhysicalMonitorArray
		);

		[DllImport("dxva2", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(
			[In] HandleRef hMonitor,
			[Out] out uint pdwNumberOfPhysicalMonitors
		);

		[DllImport("dxva2", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetCapabilitiesStringLength(
			[In] HandleRef hMonitor,
			[Out] out uint pdwCapabilitiesStringLengthInCharacters
		);

		[DllImport("dxva2", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CapabilitiesRequestAndCapabilitiesReply(
			HandleRef hMonitor,
			StringBuilder pszASCIICapabilitiesString,
			uint dwCapabilitiesStringLengthInCharacters
		);

		[DllImport("dxva2", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetMonitorCapabilities(
			[In] HandleRef hMonitor,
			[Out] out MC_CAPS pdwMonitorCapabilities,
			[Out] out ColorTemperatureSupported pdwSupportedColorTemperatures
		);

		[DllImport("dxva2", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetMonitorColorTemperature(
			[In] HandleRef hMonitor,
			[Out] out ColorTemperature pctCurrentColorTemperature
		);

		[DllImport("dxva2", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetMonitorBrightness(
			[In] HandleRef hMonitor,
			[Out] out uint pdwMinimumBrightness,
			[Out] out uint pdwCurrentBrightness,
			[Out] out uint pdwMaximumBrightness
		);

		[DllImport("dxva2", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetMonitorContrast(
			[In] HandleRef hMonitor,
			[Out] out uint pdwMinimumContrast,
			[Out] out uint pdwCurrentContrast,
			[Out] out uint pdwMaximumContrast
		);
	}
}