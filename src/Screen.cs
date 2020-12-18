using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace RndWallpaper
{
	//https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/Screen.cs
	// porting Screen class instead of using Forms.Screen so I don't have to include all of Forms
	public sealed class Screen
	{
		readonly IntPtr hmonitor;
		readonly Rectangle bounds;
		readonly bool primary;
		readonly string deviceName;
		readonly int bitDepth;

		const int PRIMARY_MONITOR = unchecked((int)0xBAADF00D);
		const int MONITOR_DEFAULTTONULL       = 0x00000000;
		const int MONITOR_DEFAULTTOPRIMARY    = 0x00000001;
		const int MONITOR_DEFAULTTONEAREST    = 0x00000002;
		const int MONITORINFOF_PRIMARY        = 0x00000001;
		const int SM_CMONITORS = 80;
		const int SM_CXSCREEN =  0;
		const int SM_CYSCREEN =  1;


		readonly static HandleRef NullHandleRef = new HandleRef(null, IntPtr.Zero);
		static bool multiMonitorSupport = (WindowsMethods.GetSystemMetrics(SM_CMONITORS) != 0);
		static Screen[] screens;

		internal Screen(IntPtr monitor) : this(monitor, IntPtr.Zero) {}
		internal Screen(IntPtr monitor, IntPtr hdc)
		{
			IntPtr screenDC = hdc;
			if (!multiMonitorSupport || monitor == (IntPtr)PRIMARY_MONITOR) {
				// Single monitor system
				bounds = new Rectangle(new Point(0,0),PrimaryMonitorSize);
				primary = true;
				deviceName = "DISPLAY";
			}
			else {
				var info = new MonitorInfoEx(); info.Init();
				WindowsMethods.GetMonitorInfo(new HandleRef(null, monitor), ref info);
				bounds = info.Monitor; //implicit conversion
				primary = ((info.Flags & MONITORINFOF_PRIMARY) != 0);

				deviceName = new string(info.DeviceName);
				deviceName = deviceName.TrimEnd((char)0);

				if (hdc == IntPtr.Zero) {
					screenDC = WindowsMethods.CreateDC(deviceName, null, null, IntPtr.Zero);
				}
			}
			hmonitor = monitor;

			this.bitDepth = WindowsMethods.GetDeviceCaps(new HandleRef(null, screenDC), DeviceCap.BITSPIXEL);
			this.bitDepth *= WindowsMethods.GetDeviceCaps(new HandleRef(null, screenDC), DeviceCap.PLANES);

			if (hdc != screenDC) {
				WindowsMethods.DeleteDC(new HandleRef(null, screenDC));
			}
		}

		public static Screen[] AllScreens {
			get {
				if (screens == null) {
					InitScreens();
				}
				return screens;
			}
		}

		static void InitScreens()
		{
			if (multiMonitorSupport) {
				MonitorEnumCallback closure = new MonitorEnumCallback();
				WindowsMethods.EnumMonitorsDelegate proc = new WindowsMethods.EnumMonitorsDelegate(closure.Callback);
				WindowsMethods.EnumDisplayMonitors(NullHandleRef, IntPtr.Zero, proc, IntPtr.Zero);

				if (closure.ScreenList.Count > 0) {
					Screen[] temp = new Screen[closure.ScreenList.Count];
					closure.ScreenList.CopyTo(temp, 0);
					screens = temp;
				}
				else {
					screens = new Screen[] { new Screen((IntPtr)PRIMARY_MONITOR) };
				}
			}
			else {
				screens = new Screen[] { new Screen((IntPtr)PRIMARY_MONITOR, IntPtr.Zero) };
			}
		}

		static Size PrimaryMonitorSize {
			get {
				int x = WindowsMethods.GetSystemMetrics(SM_CXSCREEN);
				int y = WindowsMethods.GetSystemMetrics(SM_CYSCREEN);
				return new Size(x,y);
			}
		}

		public int BitsPerPixel { get {	return bitDepth; } }
		public Rectangle Bounds { get { return bounds; } }
		public string DeviceName { get { return deviceName; } }
		public bool Primary { get { return primary; } }

		class MonitorEnumCallback {
			public List<Screen> ScreenList = new List<Screen>();

			public virtual bool Callback(IntPtr monitor, IntPtr hdc, ref RECT lprcMonitor, IntPtr lparam) {
				ScreenList.Add(new Screen(monitor, hdc));
				return true;
			}
		}
	}
}