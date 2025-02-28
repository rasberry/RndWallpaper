using System;

namespace RndWallpaper;

internal static class Log
{
	public static void Message(string m)
	{
		Console.WriteLine(m);
	}

	public static void Debug(string m)
	{
		#if DEBUG
		Console.WriteLine($"D: {m}");
		#endif
	}

	public static void Error(string m)
	{
		Console.Error.WriteLine($"E: {m}");
	}

	public static void CouldNotParse(string name, object val) {
		Log.Error($"invalid value '{val}' for '{name}'");
	}
	public static void MustProvideInput(string name) {
		Log.Error($"option '{name}' is required");
	}
	public static void MissingArgument(string name) {
		Log.Error($"not enough arguments for '{name}'");
	}
	public static void CannotFindPath(string path) {
		Log.Error($"cannot find path '{path}'");
	}
	public static void MonitorInvalid(string pick) {
		Log.Error($"Chosen monitor {pick} is invalid");
	}
	public static void NoMonitors() {
		Log.Error("No monitors were detected");
	}
	public static void SettingBackgroundDelay(int delayMS) {
		Log.Message($"setting background in {Math.Round(delayMS/1000.0,3)} seconds");
	}
	public static void SettingStyle(PickWallpaperStyle style) {
		Log.Message($"Setting style to {style}");
	}
	public static void SettingBackground(string name, int monitorIndex, string path) {
		Log.Message($"Setting {name}[{monitorIndex}] to {path}");
	}
	public static void InvalidMonitorNum(int dnum) {
		Log.Error($"Invalid monitor number '{dnum}' encountered");
	}
	public static void NoImagesFound() {
		Log.Error($"No supported images found");
	}
	public static void FormatNotSupported(string ext) {
		Log.Error($"Image format '{ext}' is not supported");
	}
	public static void MustBeGreaterThanZero(string name, double num) {
		Log.Error($"value for {name} ({num}) must be greater than zero");
	}
}