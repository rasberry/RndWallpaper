using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Windows.Storage;
using Windows.System.UserProfile;

namespace RndWallpaper
{
	public static class Helpers
	{
		public static int SetBackground(string fileName, PickWallpaperStyle style = PickWallpaperStyle.Fill)
		{
			if (!File.Exists(fileName)) { return 0; }
			switch(style)
			{
			case PickWallpaperStyle.Tile:
				SetOptions("WallpaperStyle", "0");
				SetOptions("TileWallpaper", "1");
				break;
			case PickWallpaperStyle.Center:
				SetOptions("WallpaperStyle", "0");
				SetOptions("TileWallpaper", "0");
				break;
			case PickWallpaperStyle.Stretch:
				SetOptions("WallpaperStyle", "2");
				SetOptions("TileWallpaper", "0");
				break;
			case PickWallpaperStyle.Fit:
				SetOptions("WallpaperStyle", "6");
				SetOptions("TileWallpaper", "0");
				break;
			case PickWallpaperStyle.Fill:
				SetOptions("WallpaperStyle", "10");
				SetOptions("TileWallpaper", "0");
				break;
			case PickWallpaperStyle.Span:
				SetOptions("WallpaperStyle", "22");
				SetOptions("TileWallpaper", "0");
				break;
			}

			SetOptions("Wallpaper", fileName);
			SPIF fWinIni = SPIF.UPDATEINIFILE | SPIF.SENDWININICHANGE;
			bool result = WindowsMethods.SystemParametersInfo(UAction.SPI_SETDESKWALLPAPER, 0, fileName, fWinIni);
			if (!result) {
				int error = Marshal.GetLastWin32Error();
				return error;
			}
			return 0;
		}

		public static string GetOptions(string optionsName)
		{
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", writable: true);
			if (registryKey != null)
			{
				return registryKey.GetValue(optionsName, "0").ToString();
			}
			return "0";
		}

		public static bool SetOptions(string optionsName, string optionsData)
		{
			bool result = true;
			RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", writable: true);
			if (registryKey != null)
			{
				registryKey.SetValue(optionsName, optionsData);
			}
			else
			{
				result = false;
			}
			return result;
		}

		static int NumberOfMonitors = -1;
		const int SM_CMONITORS = 80;
		public static bool MultiMonitorSupport
		{
			get {
				if (NumberOfMonitors < 0) {
					NumberOfMonitors = WindowsMethods.GetSystemMetrics(SM_CMONITORS);
				}
				return NumberOfMonitors > 0;
			}
		}

		public static int MonitorCount
		{
			get {
				if (MultiMonitorSupport) {
					return NumberOfMonitors;
				}
				return 1;
			}
		}

		public static Color ColorRefToColor(uint colorRef)
		{
			//COLORREF is 0x00bbggrr
			// https://docs.microsoft.com/en-us/windows/win32/gdi/colorref
			int b = (int)(colorRef >> 16 & 0xFF);
			int g = (int)(colorRef >> 08 & 0xFF);
			int r = (int)(colorRef >> 00 & 0xFF);
			return Color.FromArgb(r,g,b);
		}

		public static Color DwmColorToColor(uint dwmColor)
		{
			//Dwm color is aarrggbb
			// https://docs.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmgetcolorizationcolor
			int a = (int)(dwmColor >> 24 & 0xFF);
			int r = (int)(dwmColor >> 16 & 0xFF);
			int g = (int)(dwmColor >> 08 & 0xFF);
			int b = (int)(dwmColor >> 00 & 0xFF);
			return Color.FromArgb(a,r,g,b);
		}

		public static void GetMonitorInfo2()
		{
			//MONITORINFO lPrimaryScreenInfo = new MONITORINFO();
			DISPLAY_DEVICE dd=new DISPLAY_DEVICE();
			dd.cb=Marshal.SizeOf(dd);

			uint id=0;
			bool done=false;
			while(!done) {
				done = !WindowsMethods.EnumDisplayDevices(null, id, ref dd, 0);
				id++;

				if (dd.StateFlags == DisplayDeviceStateFlags.None) { continue; }
				if (!done) {
					Log.Message($"{id}\n{dd.DeviceName}\n{dd.DeviceString}\n{dd.StateFlags}\n{dd.DeviceID}\n{dd.DeviceKey}\n");
					dd.cb=Marshal.SizeOf(dd);
				}
			}
		}

		public static void GetMonitorInfo3()
		{
			var w = (IDesktopWallpaperPrivate)new DesktopWallpaperClass();
			uint count = w.GetMonitorDevicePathCount();
			for(uint m=0; m<count; m++) {
				string mpath = w.GetMonitorDevicePathAt(m);
				int num = w.GetMonitorNumber(mpath);
				Log.Message($"{m} path={mpath} num={num}");
			}
		}

		public static void GetMonitorInfo1()
		{
			var all = Screen.AllScreens;
			Log.Message($"screen count = {all.Length}");
			for(int i=0; i<all.Length; i++) {
				var s = all[i];
				Log.Message($"{s.Primary}\n{s.BitsPerPixel}\n{s.Bounds}\n{s.DeviceName}\n{s.WorkingArea}");
			}
		}

		public static bool IsExtensionSupported(string ext)
		{
			switch(ext) {
				case ".bmp":  case ".dib":  case ".gif":
				case ".jfif": case ".jpe":  case ".jpeg":
				case ".jpg":  case ".png":  case ".tif":
				case ".tiff": case ".wdp":
					return true;
			}
			return false;
		}

		public static IDesktopWallpaperPrivate GetWallPaperInstance()
		{
			var wp = (IDesktopWallpaperPrivate)new DesktopWallpaperClass();
			return wp;
		}

		public static DesktopWallpaperPosition MapStyle(PickWallpaperStyle style)
		{
			switch(style) {
			case PickWallpaperStyle.Center:  return DesktopWallpaperPosition.Center;
			case PickWallpaperStyle.Fill:    return DesktopWallpaperPosition.Fill;
			case PickWallpaperStyle.Fit:     return DesktopWallpaperPosition.Fit;
			case PickWallpaperStyle.Span:    return DesktopWallpaperPosition.Span;
			case PickWallpaperStyle.Stretch: return DesktopWallpaperPosition.Stretch;
			case PickWallpaperStyle.Tile:    return DesktopWallpaperPosition.Tile;
			}
			return DesktopWallpaperPosition.Fill;
		}

	}
}