using System;
using System.Collections.Generic;

namespace RndWallpaper;

public enum PickDevice
{
	None = 0,
	WindowsDesktop = 1,
	VirtualWindows = 2 //V first so user can specify just 'V'
}

public enum PickWallpaperStyle
{
	None = 0,
	Center,
	Tile,
	Stretch,
	Fit,
	Fill,
	Span = 6
}

//special enum used to select monitor
public enum PickMonitor
{
	Name = -1,
	Primary = -2,
	All = -3,
}

public interface IDevice : IDisposable
{
	IReadOnlyList<IMonitor> AllMonitors { get; }
	void SetWallPaperAll(string imagePath);
	void SetWallPaper(IMonitor m, string imagePath);
	void SetStyle(PickWallpaperStyle s);
	string GetWallPaper(IMonitor m);
}

public interface IMonitor
{
	System.Drawing.Rectangle? Dimensions { get; }
	string Name { get; }
	bool IsPrimary { get; }
	int? BitsPerPixel { get; }
	int? PhysicalWidthMm { get; }
	int? PhysicalHeightMm { get; }
	int? VerticalRefresh { get; }
}

