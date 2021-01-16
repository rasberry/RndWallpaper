using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RndWallpaper
{
	class Program
	{
		[STAThread]
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
				Options.Usage();
				return;
			}
			if (!Options.ParseArgs(args)) {
				return;
			}

			using(var wp = new DesktopWallpaperClass()) {
				ChangeBackground(wp);
			}
		}

		static void ChangeBackground(DesktopWallpaperClass wp)
		{
			uint dcount = Helpers.GetMonitorCount(wp);
			if (dcount < 1) {
				Log.NoMonitors();
				return;
			}

			if (!FillPlan(dcount,out string[] thePlan)) {
				return;
			}

			if (Options.DelayMS > 0) {
				Log.SettingBackgroundDelay(Options.DelayMS);
				System.Threading.Thread.Sleep(Options.DelayMS);
			}

			Log.SettingStyle(Options.Style);
			wp.SetPosition(Helpers.MapStyle(Options.Style));

			//span uses only the first image
			if (Options.Style == PickWallpaperStyle.Span) {
				Log.SettingBackground(1,thePlan[0]);
				wp.SetWallpaper(null,thePlan[0]);
			}
			//all monitors
			else if (Options.Monitor == PickMonitor.All) {
				for(uint m=0; m<dcount; m++) {
					if (thePlan[m] == null) { continue; }
					string mname = wp.GetMonitorDevicePathAt(m);
					Log.SettingBackground(m + 1,thePlan[m]);
					wp.SetWallpaper(mname,thePlan[m]);
				}
			}
			//selected monitor
			else {
				int dnum = wp.GetMonitorNumber(Options.MonitorId);
				if (dnum < 1 || dnum > dcount) {
					Log.InvalidMonitorNum(dnum);
				}
				string file = thePlan[dnum - 1];
				Log.SettingBackground((uint)dnum,file);
				wp.SetWallpaper(Options.MonitorId,file);
			}
		}

		static bool FillPlan(uint monitorCount, out string[] thePlan)
		{
			thePlan = new string[monitorCount];

			if (Options.IsFolder) {
				var rawList = Directory.GetFileSystemEntries(Options.PicPath);
				var imgList = rawList.Where(file => {
					var norm = file.ToLowerInvariant();
					string ext = Path.GetExtension(norm);
					return Helpers.IsExtensionSupported(ext);
				}).ToList();

				int count = imgList.Count;
				if (count < 1) {
					Log.NoImagesFound(Options.PicPath);
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
			}
			else {
				var norm = Options.PicPath.ToLowerInvariant();
				string ext = Path.GetExtension(norm);
				if (!Helpers.IsExtensionSupported(ext)) {
					Log.FormatNotSupported(ext);
					return false;
				}

				for(int i=0; i<monitorCount; i++) {
					thePlan[i] = Options.PicPath;
				}
			}

			if (Options.DetectPanorama) {
				//TODO need to check dimensions thePlan[0] and see if it's greater than PanoramaRatio
			}

			return true;
		}
	}
}
