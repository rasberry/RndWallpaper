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