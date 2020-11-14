using System;
using System.Drawing;
using System.Threading.Tasks;

namespace RndWallpaper
{
	class Program
	{
		static void Main(string[] args)
		{
			if (!ExecutionMode.IsRunningWithIdentity()) {
			}


			var c = Helpers.GetAccentColor();
			Console.WriteLine($"accent = {c}");

			var r = Helpers.SetWallpaperAsync(args[0]).Result;
			Console.WriteLine("result = "+r);

			//Helpers.DwmGetColorizationColor(out uint ucolor,out bool doBlend);
			//Color c = Helpers.DwmColorToColor(ucolor);
			//Console.WriteLine($"color is {c} {ucolor,0:X} doBlend {doBlend}");

			//Helpers.GetUserColorPreference(out Helpers.ImmersiveColorPreference pref, false);
			//Color c1 = Helpers.ColorRefToColor(pref.Color1);
			//Color c2 = Helpers.ColorRefToColor(pref.Color2);
			//Console.WriteLine($"pref 1={c1} {pref.Color1,0:X} 2={c2} {pref.Color2,0:X}");
		}

	}
}

/*
a029cc

https://github.com/m417z/Windows-10-Color-Control/blob/master/WindowsThemeColorApi.cpp
uxtheme.dll

 static HRESULT(WINAPI *GetUserColorPreference)(IMMERSIVE_COLOR_PREFERENCE *pcpPreference, bool fForceReload);
 static HRESULT(WINAPI *SetUserColorPreference)(const IMMERSIVE_COLOR_PREFERENCE *cpcpPreference, bool fForceCommit);

struct IMMERSIVE_COLOR_PREFERENCE {
	COLORREF color1;
	COLORREF color2;
};

COLORREF 32 bit unsigned
*/

/*

https://stackoverflow.com/questions/1061678/change-desktop-wallpaper-using-code-in-net


public sealed class Wallpaper
{
	Wallpaper() { }

	const int SPI_SETDESKWALLPAPER = 20;
	const int SPIF_UPDATEINIFILE = 0x01;
	const int SPIF_SENDWININICHANGE = 0x02;

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

	public enum Style : int
	{
		Tiled,
		Centered,
		Stretched
	}

	public static void Set(Uri uri, Style style)
	{
		System.IO.Stream s = new System.Net.WebClient().OpenRead(uri.ToString());

		System.Drawing.Image img = System.Drawing.Image.FromStream(s);
		string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
		img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

		RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
		if (style == Style.Stretched)
		{
			key.SetValue(@"WallpaperStyle", 2.ToString());
			key.SetValue(@"TileWallpaper", 0.ToString());
		}

		if (style == Style.Centered)
		{
			key.SetValue(@"WallpaperStyle", 1.ToString());
			key.SetValue(@"TileWallpaper", 0.ToString());
		}

		if (style == Style.Tiled)
		{
			key.SetValue(@"WallpaperStyle", 1.ToString());
			key.SetValue(@"TileWallpaper", 1.ToString());
		}

		SystemParametersInfo(SPI_SETDESKWALLPAPER,
			0,
			tempPath,
			SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
	}
}



// https://docs.microsoft.com/en-us/uwp/api/Windows.System.UserProfile.UserProfilePersonalizationSettings?view=winrt-19041

using Windows.System.UserProfile;

// Pass in a relative path to a file inside the local appdata folder
async Task<bool> SetWallpaperAsync(string localAppDataFileName)
{
	bool success = false;
	if (UserProfilePersonalizationSettings.IsSupported())
	{
		var uri = new Uri("ms-appx:///Local/" + localAppDataFileName);
		StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
		UserProfilePersonalizationSettings profileSettings = UserProfilePersonalizationSettings.Current;
		success = await profileSettings.TrySetLockScreenImageAsync(file);
	}
	return success;
}


*/