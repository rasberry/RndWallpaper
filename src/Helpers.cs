using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using RndWallpaper.Windows;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles.Icc;

namespace RndWallpaper;

public static class Helpers
{
	public static IEnumerable<PickDevice> GetAvailablePlatformDevices()
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
			if (Environment.OSVersion.Version.Build >= 22000) {
				yield return PickDevice.VirtualWindows;
			}
			yield return PickDevice.WindowsDesktop;
		}
	}

	public static bool IsFileSupported(string file)
	{
		var norm = file.ToLowerInvariant();
		string ext = Path.GetExtension(norm);
		return IsExtensionSupported(ext);
	}

	//TOOD maybe move this to the device ?
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

	public static IEnumerable<(string,string)> GetImageTags(string imagePath, ImageInfo info)
	{
		var meta = info.Metadata;
		//Log.Debug($"meta={meta}");

		// string ext = Path.GetExtension(imagePath).ToLowerInvariant();
		// if (ext == ".png") {
		// 	var data = info.Metadata.GetPngMetadata();
		// 	if (data != null) {
		// 		foreach(var kvp in data.TextData) {
		// 			yield return (kvp.Keyword,kvp.Value);
		// 		}
		// 	}
		// }
		// else if (ext == ".gif") {
		// 	var data = info.Metadata.GetGifMetadata();
		// 	foreach(string c in data.Comments) {
		// 		yield return (c,"");
		// 	}
		// }

		var exif = meta.ExifProfile;
		if (exif != null) {
			foreach(var val in exif.Values) {
				yield return ($"EXIF.{val.Tag}",ExifValueToString(val));
			}
		}

		var iccp = meta.IccProfile;
		if (iccp != null) {
			var vals = DecodeIccProfile(iccp);
			foreach(var (n,v) in vals) {
				yield return (n,v);
			}
		}

		var iptc = meta.IptcProfile;
		if (iptc != null) {
			foreach(var val in iptc.Values) {
				yield return ($"IPTC.{val.Tag}",val.Value);
			}
		}

		// var xmpp = meta.XmpProfile;
		// if (xmpp != null) {
		// 	var doc = xmpp.GetDocument();

		// }

		// var cicp = meta.CicpProfile;
		// if (cicp != null) {
		// 	cicp.ColorPrimaries
		// }
	}

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
				return BytesToString(data);
			}

			return $"[not implemented. type:{v.DataType}]";
		}
		else {
			return v.GetValue().ToString();
		}
	}

	static IEnumerable<(string,string)> DecodeIccProfile(IccProfile iccp)
	{
		var h = iccp.Header;
		yield return ("ICC.Class",h.Class.ToString());
		yield return ("ICC.CmmType", h.CmmType);
		yield return ("ICC.CreationDate",h.CreationDate.ToString("O"));
		yield return ("ICC.CreatorSignature",h.CreatorSignature);
		yield return ("ICC.DataColorSpace",h.DataColorSpace.ToString());
		yield return ("ICC.DeviceAttributes",h.DeviceAttributes.ToString());
		yield return ("ICC.DeviceManufacturer",h.DeviceManufacturer.ToString("X2"));
		yield return ("ICC.DeviceModel",h.DeviceModel.ToString("X2"));
		yield return ("ICC.FileSignature",h.FileSignature);
		yield return ("ICC.Flags",h.Flags.ToString());
		yield return ("ICC.Id",h.Id.ToString());
		yield return ("ICC.PcsIlluminant",h.PcsIlluminant.ToString());
		yield return ("ICC.PrimaryPlatformSignature",h.PrimaryPlatformSignature.ToString());
		yield return ("ICC.ProfileConnectionSpace",h.ProfileConnectionSpace.ToString());
		yield return ("ICC.RenderingIntent",h.RenderingIntent.ToString());
		yield return ("ICC.Size",h.Size.ToString());
		yield return ("ICC.Version", h.Version.ToString());

		//TODO too much work to decode these for now
		// foreach(var e in iccp.Entries) {
		// 	yield return ($"ICC.{e.TagSignature}",e.ToString());
		// }
	}

	const int MaxBytesToShow = 16;
	static string BytesToString(byte[] data)
	{
		int max = Math.Min(data.Length,MaxBytesToShow);
		string hasMore = data.Length > MaxBytesToShow ? " ..." : "";
		string hex = BitConverter.ToString(data,0,max).Replace("-"," ");
		return $"Bytes [{hex}{hasMore}]";
	}

	public static bool TrySelectMonitorByName(IDevice device, PickMonitor pick, string name, out int index)
	{
		var all = device.AllMonitors;
		index = -1;

		//user specified 'primary'
		if (pick == PickMonitor.Primary) {
			for(int m=0; m<all.Count; m++) {
				if (pick == PickMonitor.Primary && all[m].IsPrimary) {
					index = m;
					break;
				}
			}
		}
		//user specified a unkown string
		else if (pick == PickMonitor.Name) {
			bool found = false;
			for(int m = 0; m < all.Count; m++) {
				var mon = all[m];
				string monName = mon.Name;
				if (monName.Contains(name,StringComparison.InvariantCultureIgnoreCase)) {
					if (found) {
						Log.MonitorInvalid(name);
						return false;
					}
					index = m;
					found = true;
				}
			}
			return found;
		}

		// user specified a number zero or bigger; interpret as an index
		int iPick = (int)pick;
		if (iPick >= 0) {
			if (iPick > all.Count) {
				Log.InvalidMonitorNum(iPick);
				return false;
			}
			index = iPick;
			return true;
		}

		//nothing worked
		return false;
	}
}

