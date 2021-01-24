using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RndWallpaper
{
	public static class Options
	{
		public static void Usage()
		{
			var sb = new StringBuilder();
			sb.WL(0,$"{nameof(RndWallpaper)} (action) [options]");
			sb.WL();
			sb.WL(0,"Available Actions:");
			sb.PrintEnum<PickAction>(1,MapActionDesc,MapActionName);
			sb.WL();
			sb.WL(0,$"{PickAction.Wallpaper} [options] (file or folder) [ .. additional files and/or folders .. ]");
			sb.WL(1,"-d  (integer)" ,"Delay number of seconds (default 0)");
			sb.WL(1,"-s  (style)"   ,"Style of wallpaper (default 'Fill')");
			sb.WL(1,"-r"            ,"Include subdirectories when folder is specified");
			sb.WL(1,"-sa [ratio]"   ,"Detect panorama images when w/h > ratio (default 2.0)");
			sb.WL(1,"-m  (monitor)" ,"Apply image only to a single monitor");
			sb.WL(1,"-rs (integer)" ,"Random seed value (default system suplied)");
			sb.WL(1,"-fs"           ,"When folder given, use same image for all monitors");
			sb.WL();
			sb.WL(0,"Available Styles:");
			sb.PrintEnum<PickWallpaperStyle>(1);
			sb.WL();
			sb.WL(0,"Available Monitors:");
			sb.PrintMonitors(1);
			sb.WL();
			sb.WL(0,$"{PickAction.Info} [options] (file or folder) [ .. additional files and/or folders .. ]");
			sb.WL(1,"-m" ,"Show monitor information");
			sb.WL(1,"-w" ,"Show current wallpapers");
			sb.WL(1,"-i" ,"Show list of files found in given files / folders");
			sb.WL(1,"-r" ,"Include subdirectories when folder is specified");
			sb.WL();
			sb.WL(0,$"{PickAction.Download} [options] (pre-defined source or configuration file)");
			sb.WL(1,"-o (folder)" ,"Output folder for download file (default current folder)");
			sb.WL();
			sb.WL(0,"Available Sources:");
			sb.PrintEnum<PickSource>(1,MapSourceDesc);

			Log.Message(sb.ToString());
		}

		static string MapActionDesc(PickAction act)
		{
			switch(act) {
			case PickAction.Wallpaper: return "Change the wallpaper";
			case PickAction.Download:  return "Download an image from a configured source";
			case PickAction.Info:      return "Show information related to wallpaper";
			}
			return null;
		}
		static string MapActionName(PickAction act)
		{
			switch(act) {
			case PickAction.Wallpaper: return "(W)allpaper";
			case PickAction.Download:  return "(D)ownload";
			case PickAction.Info:      return "(I)nfo";
			}
			return null;
		}

		static string MapSourceDesc(PickSource src)
		{
			switch(src) {
			case PickSource.Bing:     return "Bing image of the day api";
			case PickSource.RSS:      return "RSS image feed";
			case PickSource.Windows:  return "Windows wallpaper archive";
			}
			return null;
		}

		static bool SelectMonitor(PickMonitor pickMon)
		{
			var all = Screen.AllScreens;
			for(int m=0; m<all.Length; m++) {
				if (pickMon == PickMonitor.Primary && all[m].Primary) {
					MonitorId = all[m].DeviceName;
					pickMon = (PickMonitor)(m + 1); //use the real value now
					break;
				}
				if ((int)pickMon - 1 == m) {
					//only setting this to indicate that we found something
					MonitorId = all[m].DeviceName;
				}
			}
			if (MonitorId == null) {
				Log.MonitorInvalid(pickMon);
				return false;
			}

			using(var wp = new DesktopWallpaperClass()) {
				uint dcount = Helpers.GetMonitorCount(wp);
				for(uint m=0; m<dcount; m++) {
					string dname = wp.GetMonitorDevicePathAt(m);
					int dnum = wp.GetMonitorNumber(dname); // undocumented :-o
					if (dnum < 1 || dnum > dcount) {
						Log.InvalidMonitorNum(dnum);
						return false;
					}
					if (dnum == (int)pickMon) {
						MonitorId = dname;
						break;
					}
				}
			}

			return true;
		}

		public static bool ParseArgs(string[] args)
		{
			var p = new Params(args);
			if (p.Expect<PickAction>(out SelectedAction,"action",
				OptionHelpers.TryParseEnumFirstLetter).IsBad()) {
				return false;
			}

			switch(SelectedAction) {
				case PickAction.Wallpaper: return ParseWallpaper(p);
				case PickAction.Info:      return ParseInfo(p);
				case PickAction.Download:  return ParseDownload(p);
			}
			return false;
		}

		static bool ParseWallpaper(Params p)
		{
			if (p.Default("-d", out double delaySec).IsInvalid()) {
				return false;
			}
			DelayMS = (int)Math.Round(delaySec * 1000.0,3);
			if (p.Default("-s",out Style, PickWallpaperStyle.Fill).IsInvalid()) {
				return false;
			}
			var mParser = new Params.Parser<PickMonitor>(OptionHelpers.TryParseMonitor);
			if (p.Default("-m",out Monitor, PickMonitor.All, mParser).IsInvalid()) {
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

			if (!SlurpFileFolders(p, true)) {
				return false;
			}

			//figure out which monitor device to use
			if (Monitor != PickMonitor.All) {
				if (!SelectMonitor(Monitor)) {
					return false;
				}
			}

			return true;
		}

		static bool ParseInfo(Params p)
		{
			ShowMonitorInfo = p.Has("-m").IsGood();
			ShowWallInfo =    p.Has("-w").IsGood();
			ShowFileInfo =    p.Has("-i").IsGood();
			RecurseFolder =   p.Has("-r").IsGood();

			if (!SlurpFileFolders(p)) {
				return false;
			}

			return true;
		}

		static bool ParseDownload(Params p)
		{
			if (p.Default("-o",out OutputLocation,null).IsInvalid()) {
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

		public static PickAction SelectedAction = PickAction.None;
		public static PickWallpaperStyle Style = PickWallpaperStyle.None;
		public static int? RndSeed = null;
		public static List<string> PicPaths = new List<string>();
		public static int DelayMS = 0;
		public static string MonitorId = null;
		public static bool UseSameImage = false;
		public static PickMonitor Monitor = PickMonitor.All;
		public static bool DetectPanorama = false;
		public static double PanoramaRatio = 2.0;
		public static bool RecurseFolder = false;
		public static bool ShowMonitorInfo = false;
		public static bool ShowWallInfo = false;
		public static bool ShowFileInfo = false;
		public static string OutputLocation = null;
	}
}