using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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