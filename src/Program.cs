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

			string file = null;
			if (IsFolder) {
				var rawList = Directory.GetFileSystemEntries(PicPath);
				var imgList = rawList.Where(file => {
					var norm = file.ToLowerInvariant();
					if (norm.EndsWith(".png")) { return true; }
					if (norm.EndsWith(".jpg")) { return true; }
					return false;
				});

				int count = imgList.Count();
				if (count < 1) {
					Log.Error($"No images found in {PicPath}");
					return;
				}

				var rnd = RndSeed.HasValue ? new Random(RndSeed.Value) : new Random();
				int index = rnd.Next(count);

				file = imgList.ElementAt(index);
			}
			else {
				file = PicPath;
			}

			if (DelayMS > 0) {
				Log.Message($"setting background in {Math.Round(DelayMS/1000.0,3)} seconds");
				System.Threading.Thread.Sleep(DelayMS);
			}
			Log.Message($"setting background to {file}");
			//int result = Helpers.SetBackground(file, Style);
			var wallpaper = (IDesktopWallpaper)(new DesktopWallpaperClass());
			uint mcount = wallpaper.GetMonitorDevicePathCount();

			for(uint m=0; m<mcount; m++) {
				string mname = wallpaper.GetMonitorDevicePathAt(m);
				Log.Message($"M {m} = {mname}");
			}

			//if (result != 0) {
			//	Log.Error("setting background failed");
			//}

			//var c = Helpers.GetAccentColor();
			//Console.WriteLine($"accent = {c}");
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
			if (p.Default("-m",out PickMonitor pickMon, PickMonitor.All, mParser).IsInvalid()) {
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
			if (pickMon != PickMonitor.All) {
				var all = Screen.AllScreens;
				for(int m=0; m<all.Length; m++) {
					if (pickMon == PickMonitor.Primary && all[m].Primary) {
						MonitorDevice = all[m].DeviceName;
						break;
					}
					if ((int)pickMon - 1 == m) {
						MonitorDevice = all[m].DeviceName;
					}
				}
				if (MonitorDevice == null) {
					Log.MonitorInvalid(pickMon);
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
		static string MonitorDevice = null;
		static bool UseSameImage = false;

	}
}
