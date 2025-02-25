using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RndWallpaper.Windows;
using SixLabors.ImageSharp;

namespace RndWallpaper;

class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		//Test(); return;

		try {
			MainMain(args);
		}
		catch(Exception e) {
			Log.Error(Options.ExtraDebugInfo
				? e.ToString()
				: e.Message
			);
		}
	}

	static void MainMain(string[] args)
	{
		if (args.Length < 1) {
			Options.Usage();
			return;
		}
		if (!Options.ParseArgs(args)) {
			return;
		}

		using var cache = new DeviceCache();
		if (Options.ShowInfo) {
			ShowInfo(cache);
			return;
		}

		if (Options.DelayMS > 0) {
			Log.SettingBackgroundDelay(Options.DelayMS);
			System.Threading.Thread.Sleep(Options.DelayMS);
		}

		var device = cache.GetDevice(Options.SelectedDevice);
		ChangeBackground(device);
	}

	static void ChangeBackground(IDevice device)
	{
		int dcount = device.AllMonitors.Count;
		if (dcount < 1) {
			Log.NoMonitors();
			return;
		}

		if (!FillPlan(dcount,out string[] thePlan)) {
			return;
		}

		if (Options.DetectPanorama) {
			var info = Image.Identify(thePlan[0]);
			double iRatio = (double)info.Width / info.Height;
			// Log.Debug($"ratio is {iRatio} vs {Options.PanoramaRatio}");
			if (iRatio > Options.PanoramaRatio) {
				Options.Style = PickWallpaperStyle.Span;
			}
		}

		//span uses only the first image
		if (Options.Style == PickWallpaperStyle.Span) {
			Log.SettingStyle(Options.Style);
			device.SetStyle(Options.Style);

			Log.SettingBackground("Span",0,thePlan[0]);
			device.SetWallPaperAll(thePlan[0]);
		}
		//all monitors
		else if (Options.Monitor == PickMonitor.All) {
			Log.SettingStyle(Options.Style);
			device.SetStyle(Options.Style);

			for(int m = 0; m<dcount; m++) {
				if (thePlan[m] == null) { continue; }
				var mon = device.AllMonitors[m];
				Log.SettingBackground(mon.Name,m,thePlan[0]);
				device.SetWallPaper(mon, thePlan[m]);
			}
		}
		//selected monitor
		else {
			if (!Helpers.TrySelectMonitorByName(device, Options.Monitor, Options.MonitorName, out var index)) {
				return;
			}
			
			if (index < 0 || index >= dcount) {
				Log.InvalidMonitorNum(index);
			}
			var mon = device.AllMonitors[index];
			var file = thePlan[index];
			Log.SettingBackground(mon.Name,index,file);
			device.SetWallPaper(mon,file);
		}
	}

	static bool FillPlan(int monitorCount, out string[] thePlan)
	{
		thePlan = new string[monitorCount];
		var list = Options.PicPaths;
		var imgList = new List<string>();

		var op = new EnumerationOptions {
			RecurseSubdirectories = Options.RecurseFolder
		};

		//go backwards so we can append items
		foreach(string path in list) {
			if (Directory.Exists(path)) {
				//expand folder into files
				var rawList = Directory.GetFiles(path,"*.*",op);
				var fileList = rawList.Where(file => Helpers.IsFileSupported(file));
				imgList.AddRange(fileList);
			}
			else {
				//already a file so just check if it's supported
				if (!Helpers.IsFileSupported(path)) {
					string ext = Path.GetExtension(path);
					Log.FormatNotSupported(ext);
					return false;
				}
				imgList.Add(path);
			}
		}

		int count = imgList.Count;
		if (count < 1) {
			Log.NoImagesFound();
			return false;
		}

		var rnd = Options.RndSeed.HasValue ? new Random(Options.RndSeed.Value) : new Random();
		int index = 0;
		for(int i=0; i<monitorCount; i++) {
			if (!Options.UseSameImage || i == 0) {
				index = rnd.Next(count);
			}
			thePlan[i] = imgList[index];
		}

		return true;
	}

	const int InfoOffset = 30;
	static void ShowInfo(DeviceCache cache)
	{
		//bool isFirst = true;
		var sb = new StringBuilder();
		sb.WLOffset(InfoOffset);
		//if (Options.ShowInfo.HasFlag(Options.PickShowInfo.Monitor)) {
			sb.WL(0,"Monitor Information:");
			foreach(var (iname,dev) in LoopDevices(cache)) {
				Log.Debug($"iname={iname} dev={(dev==null?"null":"good")}");
				ShowMonitorInfo(1,dev,iname,sb);
			}
			//isFirst = false;
		//}

		// if (Options.ShowInfo.HasFlag(Options.PickShowInfo.Wall)) {
		// 	if (!isFirst) { sb.WL(); }
		// 	sb.WL(0,"Wallpaper Information:");
		// 	foreach(var (iname,dev) in LoopDevices(cache)) {
		// 		ShowWallpaperInfo(1,dev,iname,sb);
		// 	}
		// 	isFirst = false;
		// }

		sb.WLOffset();
		Log.Message(sb.ToString());
	}

	static IEnumerable<(string,IDevice)> LoopDevices(DeviceCache cache)
	{
		var platforms = Helpers.GetAvailablePlatformDevices();
		foreach(var p in platforms) {
			var dev = cache.GetDevice(p);
			if (dev == null) { continue; }
			var iname = Options.NiceDeviceName(p);
			yield return (iname,dev);
		}
	}

	static void ShowMonitorInfo(int level, IDevice device, string iname, StringBuilder sb)
	{
		var all = device.AllMonitors;
		for(int m = 0; m < all.Count; m++) {
			var current = all[m];
			string wallpaper = device.GetWallPaper(current);

			sb.WL();
			sb.WL(level,"Interface:", iname);
			sb.WL(level,"Device Name:", current.Name);
			sb.WL(level,"Device Index:", m.ToString());
			sb.WL(level,"Is Primary:", current.IsPrimary ? "Yes" : "No");
			if (current.BitsPerPixel != null) {
				sb.WL(level,"Bits per pixel:", current.BitsPerPixel);
			}
			if (current.Dimensions != null) {
				var dim = current.Dimensions.Value;
				sb.WL(level,"Width:", dim.Width);
				sb.WL(level,"Height:", dim.Height);
				sb.WL(level,"X Offset:", dim.X);
				sb.WL(level,"Y Offset:", dim.Y);
			}
			if (current.PhysicalWidthMm != null) {
				sb.WL(level,"Physical Width (mm)", current.PhysicalWidthMm);
			}
			if (current.PhysicalHeightMm != null) {
				sb.WL(level,"Physical Height (mm)", current.PhysicalHeightMm);
			}
			if (current.VerticalRefresh != null) {
				sb.WL(level,"Vertical Refresh (Hz)", current.VerticalRefresh);
			}
			sb.WL(level,"Image Path:", wallpaper);
		}
	}

	static void ShowFileInfo(StringBuilder sb, int level, string wallpaper)
	{
		if (!File.Exists(wallpaper)) {
			sb.WL(level,"[Unable to find image on disk]");
			return;
		}

		var info = Image.Identify(wallpaper);
		sb.WL(level,"Image Width:"           ,info.Width);
		sb.WL(level,"Image Height:"          ,info.Height);
		sb.WL(level,"Image Bits per pixel:"  ,info.PixelType.BitsPerPixel);

		var meta = Helpers.GetImageTags(wallpaper,info);
		if (meta.Any()) {
			sb.WL();
			sb.WL(level,"Metadata:");

			foreach(var kvp in meta) {
				sb.WL(level + 1,kvp.Item1,kvp.Item2);
			}
		}
	}

	#if DEBUG
	static void Test()
	{

		var sb = new StringBuilder();
		ShowFileInfo(sb,0,"D:\\Anders\\Pictures\\Backgrounds\\abstract\\gXLcYNz - Imgur.jpg");
		Log.Message(sb.ToString());
	}

	static void TestPrintMeta()
	{
		foreach(string p in Options.PicPaths) {
			if (!File.Exists(p)) {
				Log.Message($"{p} does not exist");
				continue;
			}
			Log.Message($"# file {p}");

			var info = Image.Identify(p);
			if (info == null) { continue; }
			Log.Message($"dims are {info.Width}x{info.Height}");
			var meta = Helpers.GetImageTags(p,info);
			StringBuilder sb = new StringBuilder();

			if (meta.Any()) {
				sb.WL(0,"Metadata:");
				foreach(var kvp in meta) {
					sb.WL(1,kvp.Item1,kvp.Item2);
				}
			}

			Log.Message(sb.ToString());
		}
	}
	#endif
}
