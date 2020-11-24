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
		//public static void UnknownAction(object val) {
		//	Log.Error($"unkown action '{val}'");
		//}
		//public static void ActionNotSpecified() {
		//	Log.Error("action was not specified");
		//}
		public static void MustProvideInput(string name) {
			Log.Error($"option '{name}' is required");
		}
		public static void MissingArgument(string name) {
			Log.Error($"not enough arguments for '{name}'");
		}
		//public static void MustHaveOnePriority() {
		//	Log.Error("You must provide at least one priority");
		//}
		//public static void PriorityMustBeNumber() {
		//	Log.Error("Each priority must be a number");
		//}
		//public static void MustBeGreaterThanZero(string name, bool includeZero = false) {
		//	Log.Error($"{name} must be greater than {(includeZero?"or equal to ":"")}zero");
		//}
		//public static void MustBeBetween(string name, string low, string high) {
		//	Log.Error($"{name} must be between {low} and {high}");
		//}
		public static void CannotFindPath(string path) {
			Log.Error($"cannot find path '{path}'");
		}
		//public static void MustBeSizeInBytes(string name, int sizeInBytes, bool isMin = false) {
		//	Log.Error($"{name} must be {(isMin?"at least ":"")}{sizeInBytes} bytes");
		//}
		//public static void InvalidPassword() {
		//	Log.Error("password is missing or invalid");
		//}
	}
}