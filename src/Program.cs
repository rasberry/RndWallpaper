using System;
using System.IO;
using System.Linq;

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
			if (p.Default("-s",out Style, PickWallpaperStyle.Fill).IsInvalid()) {
				return false;
			}
			if (p.Default("-rs", out RndSeed).IsInvalid()) {
				return false;
			}
			if (p.Default("-d", out double delaySec).IsInvalid()) {
				return false;
			}
			DelayMS = (int)Math.Round(delaySec * 1000.0,3);

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

			return true;
		}

		static void Usage()
		{
			Log.Message(
				   $"{nameof(RndWallpaper)} [options] (path of image or folder)"
				+ "\nOptions:"
				+ "\n -d (number)        Delay number of seconds (default 0)"
				+ "\n -s (style)         Style of wallpaper (default 'Fill')"
				+ "\n -rs (integer)      Random seed value (default system suplied)"
				+ "\n -op                Apply image only to the primary monitor"
				+ "\n -om (name)         Apply image to monitor with given name"
				+ "\n -os                Apply the same image to all monitors (if folder given)"
				+ "\n -?? (todo figure out what options to use for this"
				// + "\n -ta (boolean)      Enable or disable updating accent with the background (default leave as-is)"
				+ "\n"
				+ "\nAvailable Styles:"
				+ "\n Center"
				+ "\n Stretch"
				+ "\n Fit"
				+ "\n Fill"
				+ "\n Span"
			);
		}

		static PickWallpaperStyle Style = PickWallpaperStyle.None;
		static int? RndSeed = null;
		static string PicPath = null;
		static bool IsFolder = false;
		static int DelayMS = 0;

	}
}
