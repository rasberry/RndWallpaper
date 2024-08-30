using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RndWallpaper;

public static class Options
{
	public static void Usage()
	{
		var sb = new StringBuilder();
		sb.WL(0,$"{nameof(RndWallpaper)} [options] (file or folder) [ .. additional files and/or folders .. ]");
		sb.WL();
		sb.WL(0,"Informational:");
		sb.WL(1,"-i"  ,"Show monitor information");
		sb.WL();
		sb.WL(0,"Wallpaper:");
		sb.WL(1,"-d  (interface)" ,"Choose which interface to use");
		sb.WL(1,"-w  (integer)"   ,"Delay number of seconds (default 0)");
		sb.WL(1,"-s  (style)"     ,"Style of wallpaper (default 'Fill')");
		sb.WL(1,"-r"              ,"Include subdirectories when folder is specified");
		sb.WL(1,"-sa [ratio]"     ,"Detect panorama images when w/h > ratio (default 2.0)");
		sb.WL(1,"-m  (monitor)"   ,"Apply image only to a single monitor");
		sb.WL(1,"-rs (integer)"   ,"Random seed value (default system suplied)");
		sb.WL(1,"-fs"             ,"When folder given, use same image for all monitors");
		sb.WL();
		sb.WL(0,"Available Styles:");
		sb.PrintEnum<PickWallpaperStyle>(1);
		sb.WL();
		sb.WL(0,"Available Interfaces:");
		sb.PrintListWithBullets(PopDevices(),1);


		Log.Message(sb.ToString());
	}

	static List<(int, string, string)> PopDevices()
	{
		var list = new List<(int, string, string)>();
		foreach(var item in Helpers.GetAvailablePlatformDevices()) {
			int num = (int)item;
			string name = MapDeviceName(item) ?? "";
			string desc = MapDeviceDesc(item) ?? "";
			list.Add((num,name,desc));
		}
		return list;
	}

	static string MapDeviceDesc(PickDevice act)
	{
		switch(act) {
		case PickDevice.WindowsDesktop: return "Change wallpaper using Windows legacy api";
		case PickDevice.VirtualWindows: return "Change wallpaper for Windows Virtual Desktop";
		}
		return null;
	}
	static string MapDeviceName(PickDevice act)
	{
		switch(act) {
		case PickDevice.WindowsDesktop: return "(W)indows Legacy";
		case PickDevice.VirtualWindows: return "Windows (V)irtual Desktop";
		}
		return null;
	}
	public static string NiceDeviceName(PickDevice act)
	{
		switch(act) {
		case PickDevice.WindowsDesktop: return "Windows Legacy Api";
		case PickDevice.VirtualWindows: return "Windows Virtual Desktop Api";
		}
		return null;
	}

	// static bool SelectMonitor(PickMonitor pickMon)
	// {
	// 	var all = Screen.AllScreens;
	// 	for(int m=0; m<all.Length; m++) {
	// 		if (pickMon == PickMonitor.Primary && all[m].Primary) {
	// 			MonitorId = all[m].DeviceName;
	// 			pickMon = (PickMonitor)(m + 1); //use the real value now
	// 			break;
	// 		}
	// 		if ((int)pickMon - 1 == m) {
	// 			//only setting this to indicate that we found something
	// 			MonitorId = all[m].DeviceName;
	// 		}
	// 	}
	// 	if (MonitorId == null) {
	// 		Log.MonitorInvalid(pickMon);
	// 		return false;
	// 	}

	// 	using(var wp = new DesktopWallpaperClass()) {
	// 		uint dcount = Helpers.GetMonitorCount(wp);
	// 		for(uint m=0; m<dcount; m++) {
	// 			string dname = wp.GetMonitorDevicePathAt(m);
	// 			int dnum = wp.GetMonitorNumber(dname); // undocumented :-o
	// 			if (dnum < 1 || dnum > dcount) {
	// 				Log.InvalidMonitorNum(dnum);
	// 				return false;
	// 			}
	// 			if (dnum == (int)pickMon) {
	// 				MonitorId = dname;
	// 				break;
	// 			}
	// 		}
	// 	}

	// 	return true;
	// }

	public static bool ParseArgs(string[] args)
	{
		var p = new Params(args);

		if (p.Has("--debug").IsGood()) {
			ExtraDebugInfo = true;
		}

		if (p.Has("-i").IsGood()) {
			ShowInfo = true;
			return true; //Stop processing options since we're only showing info
		}

		if (p.Expect<PickDevice>("-d", out SelectedDevice,
			OptionHelpers.TryParseEnumFirstLetter).IsInvalid()) {
			return false;
		}

		if (p.Default("-w", out double delaySec).IsInvalid()) {
			return false;
		}
		DelayMS = (int)Math.Round(delaySec * 1000.0,3);
		if (p.Default("-s",out Style, PickWallpaperStyle.Fill).IsInvalid()) {
			return false;
		}
		if (p.Has("-r").IsGood()) {
			RecurseFolder = true;
		}
		if (p.Default("-rs", out RndSeed).IsInvalid()) {
			return false;
		}
		if (p.Has("-sa").IsGood()) {
			DetectPanorama = true;
			//assuming bad values mean the optional parameter was not supplied
			p.Default("-sa",out PanoramaRatio,2.0,null,false);
			if (!double.IsNormal(PanoramaRatio) || PanoramaRatio < double.Epsilon) {
				Log.MustBeGreaterThanZero("-sa",PanoramaRatio);
				return false;
			}
		}
		if (p.Has("-fs").IsGood()) {
			UseSameImage = true;
		}

		//var mParser = new Params.Parser<PickMonitor>(OptionHelpers.TryParseMonitor);
		var argm = p.Default<string>("-m", out MonitorName, null);
		if (argm.IsInvalid()) {
			return false;
		}
		else if (argm.IsGood()) {
			if (SelectedDevice == PickDevice.None) {
				Log.MissingArgument("interface");
				return false;
			}

			if (!OptionHelpers.TryParseMonitor(MonitorName, out Monitor)) {
				Log.InvalidMonitorNum((int)Monitor);
			}
		}

		if (!SlurpFileFolders(p, true)) {
			return false;
		}

		return true;
	}

	static bool SlurpFileFolders(Params p, bool required = false)
	{
		//assume the rest are images or paths
		PicPaths.AddRange(p.Remaining());
		if (required && PicPaths.Count < 1) {
			Log.MustProvideInput("one of more images or folder paths");
			return false;
		}

		//check that files / folders exist
		for(int i = 0; i < PicPaths.Count; i++) {
			string full = Path.GetFullPath(PicPaths[i]);
			if (!Directory.Exists(full) && !File.Exists(full)) {
				Log.CannotFindPath(full);
				return false;
			}
			PicPaths[i] = full;
		}
		return true;
	}

	public static PickWallpaperStyle Style = PickWallpaperStyle.None;
	public static int? RndSeed = null;
	public static List<string> PicPaths = new List<string>();
	public static int DelayMS = 0;
	public static string MonitorName = null;
	public static bool UseSameImage = false;
	public static PickMonitor Monitor = PickMonitor.All;
	public static bool DetectPanorama = false;
	public static double PanoramaRatio = 2.0;
	public static bool RecurseFolder = false;
	public static string OutputLocation = null;
	public static bool ExtraDebugInfo = false;
	public static PickDevice SelectedDevice = PickDevice.None;
	public static bool ShowInfo = false;
}