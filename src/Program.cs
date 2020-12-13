using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RndWallpaper
{
	class Program
	{
		static void Main(string[] args)
		{
			try {
				MainMain(args);
			}
			catch(Exception e) {
				#if DEBUG
				Log.Error(e.ToString());
				#else
				Log.Error(e.Message);
				#endif
			}
		}

		static void MainMain(string[] args)
		{
			if (args.Length < 1) {
				Usage();
				return;
			}
			if (!ParseArgs(args)) {
				return;
			}

			var wp = (IDesktopWallpaperPrivate)(new DesktopWallpaperClass());
			uint dcount = wp.GetMonitorDevicePathCount();
			if (dcount < 1) {
				Log.NoMonitors();
				return;
			}

			if (!FillPlan(dcount,out string[] thePlan)) {
				return;
			}

			if (DelayMS > 0) {
				Log.SettingBackgroundDelay(DelayMS);
				System.Threading.Thread.Sleep(DelayMS);
			}

			Log.SettingStyle(Style);
			wp.SetPosition(Helpers.MapStyle(Style));

			if (Monitor == PickMonitor.All) {
				for(uint m=0; m<dcount; m++) {
					if (thePlan[m] == null) { continue; }
					string mname = wp.GetMonitorDevicePathAt(m);
					Log.SettingBackground(m + 1,thePlan[m]);
					wp.SetWallpaper(mname,thePlan[m]);
				}
			}
			else {
				int dnum = wp.GetMonitorNumber(MonitorId);
				if (dnum < 1 || dnum > dcount) {
					Log.InvalidMonitorNum(dnum);
				}
				string file = thePlan[dnum - 1];
				Log.SettingBackground((uint)dnum,file);
				wp.SetWallpaper(MonitorId,file);
			}
		}

		static bool FillPlan(uint monitorCount, out string[] thePlan)
		{
			thePlan = new string[monitorCount];

			if (IsFolder) {
				var rawList = Directory.GetFileSystemEntries(PicPath);
				var imgList = rawList.Where(file => {
					var norm = file.ToLowerInvariant();
					string ext = Path.GetExtension(norm);
					return Helpers.IsExtensionSupported(ext);
				}).ToList();

				int count = imgList.Count;
				if (count < 1) {
					Log.NoImagesFound(PicPath);
					return false;
				}

				var rnd = RndSeed.HasValue ? new Random(RndSeed.Value) : new Random();
				int index = 0;
				for(int i=0; i<monitorCount; i++) {
					if (!UseSameImage || i == 0) {
						index = rnd.Next(count);
					}
					thePlan[i] = imgList[index];
				}
			}
			else {
				var norm = PicPath.ToLowerInvariant();
				string ext = Path.GetExtension(norm);
				if (!Helpers.IsExtensionSupported(ext)) {
					Log.FormatNotSupported(ext);
					return false;
				}

				for(int i=0; i<monitorCount; i++) {
					thePlan[i] = PicPath;
				}
			}

			return true;
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

			var wp = Helpers.GetWallPaperInstance();
			uint dcount = wp.GetMonitorDevicePathCount();
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

			return true;
		}

		static bool ParseArgs(string[] args)
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

		static void Usage()
		{
			var sb = new StringBuilder();
			sb.WL(0,$"{nameof(RndWallpaper)} [options] (path of image or folder)");
			sb.WL(0,"Options:");
			sb.WL(1,"-d  (number)"  ,"Delay number of seconds (default 0)");
			sb.WL(1,"-s  (style)"   ,"Style of wallpaper (default 'Fill')");
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

		static PickWallpaperStyle Style = PickWallpaperStyle.None;
		static int? RndSeed = null;
		static string PicPath = null;
		static bool IsFolder = false;
		static int DelayMS = 0;
		static string MonitorId = null;
		static bool UseSameImage = false;
		static PickMonitor Monitor = PickMonitor.All;
	}
}
