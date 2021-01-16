using System;
using System.IO;
using System.Text;

namespace RndWallpaper
{
	public static class Options
	{
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
			if (p.Default("-rs", out RndSeed).IsInvalid()) {
				return false;
			}
			if (p.Has("-sa").IsGood()) {
				DetectPanorama = true;
				Style = PickWallpaperStyle.Span; //overrides -s
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

			if (p.Expect(out PicPath,"image or folder path").IsBad()) {
				return false;
			}
			//fully qualify the path
			PicPath = Path.GetFullPath(PicPath);

			if (Directory.Exists(PicPath)) {
				IsFolder = true;
			}
			else if (!File.Exists(PicPath)) {
				Log.CannotFindPath(PicPath);
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

		public static void Usage()
		{
			var sb = new StringBuilder();
			sb.WL(0,$"{nameof(RndWallpaper)} [options] (path of image or folder)");
			sb.WL(0,"Options:");
			sb.WL(1,"-d  (integer)" ,"Delay number of seconds (default 0)");
			sb.WL(1,"-s  (style)"   ,"Style of wallpaper (default 'Fill')");
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

			Log.Message(sb.ToString());
		}

		public static PickWallpaperStyle Style = PickWallpaperStyle.None;
		public static int? RndSeed = null;
		public static string PicPath = null;
		public static bool IsFolder = false;
		public static int DelayMS = 0;
		public static string MonitorId = null;
		public static bool UseSameImage = false;
		public static PickMonitor Monitor = PickMonitor.All;
		public static bool DetectPanorama = false;
		public static double PanoramaRatio = 2.0;
	}
}