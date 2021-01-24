using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SixLabors.ImageSharp;

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

			switch(Options.SelectedAction) {
				case PickAction.Wallpaper: {
					using(var wp = new DesktopWallpaperClass()) {
						ChangeBackground(wp);
					}; break;
				}
				case PickAction.Download: DownloadWallpaper(); break;
				case PickAction.Info: ShowInfo(); break;
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

			if (Options.DetectPanorama) {
				var info = Image.Identify(thePlan[0]);
				double iRatio = (double)info.Width / info.Height;
				// Log.Debug($"ratio is {iRatio} vs {Options.PanoramaRatio}");
				if (iRatio > Options.PanoramaRatio) {
					Options.Style = PickWallpaperStyle.Span;
				}
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

		static void DownloadWallpaper()
		{
		}

		static void ShowInfo()
		{
			var sb = new StringBuilder();
			if (Options.ShowMonitorInfo) {
				sb.WL(0,"Monitor Information:");
				ShowMonitorInfo(1,sb);
			}


			Log.Message(sb.ToString());
		}

		static void ShowMonitorInfo(int level, StringBuilder sb)
		{
			var all = Screen.AllScreens;
			for(int m = 0; m < all.Length; m++) {
				var current = all[m];
				sb.WL();
				sb.WL(level,"Device Name:"         ,current.DeviceName);
				sb.WL(level,"Is Primary:"          ,current.Primary ? "Yes" : "No");
				sb.WL(level,"Bits per pixel:"      ,current.BitsPerPixel.ToString());
				sb.WL(level,"Width:"               ,current.Bounds.Width.ToString());
				sb.WL(level,"Height:"              ,current.Bounds.Height.ToString());
				sb.WL(level,"X Offset:"            ,current.Bounds.X.ToString());
				sb.WL(level,"Y Offset:"            ,current.Bounds.Y.ToString());
				sb.WL(level,"Physical Width (mm)"  ,current.PhysicalWidthMm.ToString());
				sb.WL(level,"Physical Height (mm)" ,current.PhysicalHeightMm.ToString());
				sb.WL(level,"Vertical Refresh (Hz)",current.VerticalRefresh.ToString());
				sb.WL(level,"Technology"           ,current.Technology.ToString());
			}
		}
	}
}
