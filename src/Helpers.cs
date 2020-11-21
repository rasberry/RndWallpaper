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
		// https://docs.microsoft.com/en-us/uwp/api/Windows.System.UserProfile.UserProfilePersonalizationSettings?view=winrt-19041
		public static async Task<PickReason> SetWallpaperAsync(string fileName)
		{
			if (!File.Exists(fileName)) {
				return PickReason.FileDoesNotExist;
			}

			if (!UserProfilePersonalizationSettings.IsSupported()) {
				return PickReason.NotSupported;
			}

			StorageFile source = await StorageFile.GetFileFromPathAsync(fileName);
			StorageFile storage = await source.CopyAsync(ApplicationData.Current.LocalFolder);

			if (!storage.IsAvailable) {
				return PickReason.FileNotAvailable;
			}

			var profileSettings = UserProfilePersonalizationSettings.Current;
			bool success = await profileSettings.TrySetWallpaperImageAsync(storage);

			return success ? PickReason.Success : PickReason.SetWallpaperFailed;
		}

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

		public static int SetBackground(string fileName)
		{
			int result = 0;
			if (File.Exists(fileName))
			{
				string oSName = GetOSName();
				string options = GetOptions("WallpaperStyle");
				if (!options.Equals("10") && !options.Equals("22"))
				{
					if (oSName == "0" || oSName == "7" || oSName == "8" || oSName == "8.1" || MonitorCount > 1)
					{
						SetOptions("WallpaperStyle", "10");
					}
					else
					{
						SetOptions("WallpaperStyle", "22");
					}
				}
				SetOptions("TileWallpaper", "0");
				SetOptions("Wallpaper", fileName);
				StringBuilder lpvParam = new StringBuilder(fileName);
				SPIF fWinIni = SPIF.UPDATEINIFILE | SPIF.SENDWININICHANGE;
				result = SystemParametersInfo(UAction.SPI_SETDESKWALLPAPER, 0, lpvParam, fWinIni);
			}
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

		public static string GetOSName()
		{
			string text = "";
			string text2 = "";
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(
				"SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion");
			if (registryKey != null)
			{
				text = registryKey.GetValue("CurrentVersion", "").ToString();
				string text3 = registryKey.GetValue("InstallationType", "").ToString();
				text2 = registryKey.GetValue("ProductName", "").ToString();
				if (text3.Contains("Server") && text2.Contains("Server"))
				{
					return "0";
				}
				if (text.Equals("6.1"))
				{
					return "7";
				}
				if (text.Equals("6.2"))
				{
					return "8";
				}
				if (text.Equals("6.3"))
				{
					string mv = registryKey.GetValue("CurrentMajorVersionNumber", "").ToString();
					if (mv.Equals("10"))
					{
						return "10";
					}
					return "8.1";
				}
				registryKey.Close();
			}
			return "0";
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