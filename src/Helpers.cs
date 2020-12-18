using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

		#if false
		// https://github.com/dotnet/corert/issues/6252#issuecomment-415591702
		public static IDesktopWallpaperPrivate GetWallPaperInstance()
		{
			if (IdwppInstance != null) { return IdwppInstance; }

			int hr = WindowsMethods.CoInitializeEx(IntPtr.Zero, COINIT.COINIT_APARTMENTTHREADED);
			Marshal.ThrowExceptionForHR(hr, new IntPtr(-1));

			Guid ClassId = new Guid("C2CF3110-460E-4fc1-B9D0-8A1C0C9CC4BD");
			Guid InterfaceId = new Guid("C182461F-DFAC-4375-AB6E-4CC45AA7F9CC");
			//Guid InterfaceId = new Guid("B92B56A9-8B55-4E14-9A89-0199BBB6F93B");
			IntPtr ppv;

			int hResult = WindowsMethods.CoCreateInstance(
				ref ClassId, IntPtr.Zero, CLSCTX.CLSCTX_LOCAL_SERVER, ref InterfaceId, out ppv);

			Marshal.ThrowExceptionForHR(hResult, new IntPtr(-1));
			var wp = Marshal.GetTypedObjectForIUnknown(ppv, typeof(IDesktopWallpaperPrivate)) as IDesktopWallpaperPrivate;
			//Log.Debug(wp.GetType().ToString());
			IdwppInstance = wp;
			return wp;
		}

		static IDesktopWallpaperPrivate IdwppInstance = null;
		#endif

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

		/*
		public static IEnumerable<DisplayDevice> EnumerateMonitors()
		{
			DisplayDevice dd = new DisplayDevice();

			uint id=0; bool done=false;
			while(!done) {
				dd.cb = Marshal.SizeOf(dd);
				done = !WindowsMethods.EnumDisplayDevices(null, id, ref dd, 0);
				id++;

				if (dd.StateFlags == DisplayDeviceStateFlags.None) { continue; }
				yield return dd;
			}
		}
		*/

	}
}
