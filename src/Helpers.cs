using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace RndWallpaper
{
	public static class Helpers
	{
		public static bool IsFileSupported(string file)
		{
			var norm = file.ToLowerInvariant();
			string ext = Path.GetExtension(norm);
			return IsExtensionSupported(ext);
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

		public static uint GetMonitorCount(DesktopWallpaperClass wp)
		{
			uint al = (uint)Screen.AllScreens.Length;
			uint wl = wp.GetMonitorDevicePathCount();
			return Math.Min(al,wl);
		}
	}
}
