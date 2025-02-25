using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace RndWallpaper.Windows;

public sealed class DeviceWinLegacy : IDevice, IDisposable
{
	public DeviceWinLegacy()
	{
		Native = new WinDesktop();
	}

	public IReadOnlyList<IMonitor> AllMonitors { get {
		return Enumerable.Cast<IMonitor>(WinScreen.AllScreens).ToList();
	}}

	public void SetStyle(PickWallpaperStyle style)
	{
		SelectedStyle = style;
		var nativeStyle = MapStyle(style);
		Native.SetPosition(nativeStyle);
	}

	public void SetWallPaper(IMonitor m, string imagePath)
	{
		//span uses only the first image
		if (SelectedStyle == PickWallpaperStyle.Span) {
			Native.SetWallpaper(null,imagePath);
		}
		//selected monitor
		else {
			var index = ((WinScreen)m).MonitorNumber;
			string mname = Native.GetMonitorDevicePathAt((uint)index);
			Native.SetWallpaper(mname,imagePath);
		}
	}

	public void SetWallPaperAll(string imagePath)
	{
		//span uses only the first image
		if (SelectedStyle == PickWallpaperStyle.Span) {
			SetWallPaper(null, imagePath);
		}
		else {
			int dcount = WinScreen.AllScreens.Length;
			for(uint m=0; m<dcount; m++) {
				string mname = Native.GetMonitorDevicePathAt(m);
				Native.SetWallpaper(mname,imagePath);
			}
		}
	}

	public string GetWallPaper(IMonitor m)
	{
		uint dnum = (uint)((WinScreen)m).MonitorNumber;
		string mpath = Native.GetMonitorDevicePathAt(dnum);
		string wallpaper = Native.GetWallpaper(mpath);
		return wallpaper;
	}

	public void Dispose()
	{
		Native?.Dispose();
	}

	static DesktopWallpaperPosition MapStyle(PickWallpaperStyle style)
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

	readonly WinDesktop Native;
	PickWallpaperStyle SelectedStyle;
}