using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Windows.Storage;
using Windows.System.UserProfile;

namespace RndWallpaper
{
	public static class Helpers
	{
		public enum UAction
		{
			SPI_SETDESKWALLPAPER = 20,
			SPI_GETDESKWALLPAPER = 115
		}

		[Flags]
		public enum SPIF
		{
			UPDATEINIFILE = 0x01,
			SENDWININICHANGE = 0x02
		}

		[DllImport("user32", CharSet = CharSet.Unicode)]
		public static extern int SystemParametersInfo(UAction uAction, int uParam, StringBuilder lpvParam, SPIF fuWinIni);

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
			StringBuilder lpvParam = new StringBuilder(fileName);
			SPIF fWinIni = SPIF.UPDATEINIFILE | SPIF.SENDWININICHANGE;
			int result = SystemParametersInfo(UAction.SPI_SETDESKWALLPAPER, 0, lpvParam, fWinIni);
			return result;
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
					NumberOfMonitors = GetSystemMetrics(SM_CMONITORS);
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

		[DllImport("user32", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int GetSystemMetrics(int nIndex);

		public static Color GetAccentColor()
		{
			var x = new Windows.UI.ViewManagement.UISettings();
			var color = x.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
			return Color.FromArgb(color.A,color.R,color.G,color.B);
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

		[StructLayout(LayoutKind.Sequential)]
		public struct ImmersiveColorPreference
		{
			public uint Color1; //COLORREF - StartColorMenu
			public uint Color2; //COLORREF - AccentColorMenu
		}

		[DllImport("dwmapi", PreserveSig = false)]
		public static extern void DwmGetColorizationColor(out uint ColorizationColor, [MarshalAs(UnmanagedType.Bool)]out bool ColorizationOpaqueBlend);

		[DllImport("uxtheme", PreserveSig = false)]
		public static extern void GetUserColorPreference(out ImmersiveColorPreference preference, [MarshalAs(UnmanagedType.Bool)] bool forceReload);

		[DllImport("uxtheme", PreserveSig = false)]
		public static extern void SetUserColorPreference(ref ImmersiveColorPreference preference, [MarshalAs(UnmanagedType.Bool)] bool forceCommit);

	}
}