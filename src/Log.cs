using System;

namespace RndWallpaper
{
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
		public static void MonitorInvalid(PickMonitor pick) {
			Log.Error($"Chosen monitor {pick} is invalid");
		}
	}
}