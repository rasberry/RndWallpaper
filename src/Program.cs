﻿using System;
using System.IO;
using System.Linq;

namespace RndWallpaper
{
	class Program
	{
		static void Main(string[] args)
		{
			//TODO allow either directory or file to be specified
			//add actual options and usage, etc..
			//add option for wallpaper style
			//figure out what file types are supported
			//maybe add option to specify random seed .. ?
			//maybe add option to enable changing accent color setting ? (not sure if this is possible)
			//	maybe AutoColorization reg value ?
			//note: accent seems to change for me when changing background (and accent change option is enabled)
			//

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

				var rnd = RndSeed.HasValue ? new Random(RndSeed.Value) : new Random();
				int index = rnd.Next(imgList.Count());

				file = imgList.ElementAt(index);
			}
			else {
				file = PicPath;
			}

			Log.Message($"setting background to {file}");
			Helpers.SetBackground(file, Style);

			//var c = Helpers.GetAccentColor();
			//Console.WriteLine($"accent = {c}");
		}

		static bool ParseArgs(string[] args)
		{
			var p = new Params(args);
			if (p.Default("-s",out Style, PickWallpaperStyle.Fit).IsInvalid()) {
				return false;
			}
			if (p.Default("-rs", out RndSeed).IsInvalid()) {
				return false;
			}

			if (p.Expect(out PicPath,"image or folder path").IsBad()) {
				return false;
			}

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
				+ "\n -s (style)         Style of wallpaper (default 'Fit')"
				+ "\n -rs (integer)      Random seed value (default system suplied)"
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

	}
}
