using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles.Iptc;

namespace RndWallpaper
{
	public static class Helpers
	{
		public static bool IsFileSupported(string file)
		{
			var norm = file.ToLowerInvariant();
			string ext = Path.GetExtension(norm);
			return IsExtensionSupported(ext);
		}

		public static bool IsExtensionSupported(string ext)
		{
			switch(ext) {
				//taken from windows 10 wallpaper browse dialog
				case ".avci":  case ".avcs":  case ".avif":  case ".avifs":
				case ".bmp":   case ".dib":   case ".gif":   case ".heic":
				case ".heics": case ".heif":  case ".heifs": case ".jfif":
				case ".jpe":   case ".jpeg":  case ".jpg":   case ".png":
				case ".tif":   case ".tiff":  case ".wdp":
				// some others I tried that seem to work
				case ".webp":
					return true;
			}
			return false;
		}

		public static DesktopWallpaperPosition MapStyle(PickWallpaperStyle style)
		{
			switch(style) {
			case PickWallpaperStyle.Center:  return DesktopWallpaperPosition.Center;
			case PickWallpaperStyle.Fill:    return DesktopWallpaperPosition.Fill;
			case PickWallpaperStyle.Fit:     return DesktopWallpaperPosition.Fit;
			case PickWallpaperStyle.Span:    return DesktopWallpaperPosition.Span;
			case PickWallpaperStyle.Stretch: return DesktopWallpaperPosition.Stretch;
			case PickWallpaperStyle.Tile:    return DesktopWallpaperPosition.Tile;
			}
			return DesktopWallpaperPosition.Fill;
		}

		public static uint GetMonitorCount(DesktopWallpaperClass wp)
		{
			uint al = (uint)Screen.AllScreens.Length;
			uint wl = wp.GetMonitorDevicePathCount();
			return Math.Min(al,wl);
		}

		public static IEnumerable<(string,string)> GetImageTags(string imagePath, ImageInfo info)
		{
			string ext = Path.GetExtension(imagePath).ToLowerInvariant();
			if (ext == ".png") {
				var data = info.Metadata.GetPngMetadata();
				if (data != null) {
					foreach(var kvp in data.TextData) {
						yield return (kvp.Keyword,kvp.Value);
					}
				}
			}
			else if (ext == ".gif") {
				var data = info.Metadata.GetGifMetadata();
				foreach(string c in data.Comments) {
					yield return (c,"");
				}
			}

			var exif = info.Metadata.ExifProfile;
			if (exif != null) {
				foreach(var val in exif.Values) {
					yield return (val.Tag.ToString(),ExifValueToString(val));
				}
			}

			var iptc = info.Metadata.IptcProfile;
			if (iptc != null) {
				foreach(var val in iptc.Values) {
					yield return (val.Tag.ToString(),val.Value);
				}
			}
		}

		const int MaxBytesToShow = 16;
		static string ExifValueToString(IExifValue v)
		{
			if (v.Tag == ExifTag.UserComment || v.Tag == ExifTag.ExifVersion) {
				var data = v.GetValue() as byte[];
				string s = Encoding.UTF8.GetString(data);
				return s;
			}

			if (v.IsArray) {
				if (v.DataType == ExifDataType.Undefined || v.DataType == ExifDataType.Byte) {
					var data = v.GetValue() as byte[];
					int max = Math.Min(data.Length,MaxBytesToShow);
					string hasMore = data.Length > MaxBytesToShow ? " ..." : "";
					string hex = BitConverter.ToString(data,0,max).Replace("-"," ");
					string s = $"Bytes [{hex}{hasMore}]";
					return s;
				}

				return $"[not implemented. type:{v.DataType}]";
			}
			else {
				return v.GetValue().ToString();
			}
		}
	}
}
