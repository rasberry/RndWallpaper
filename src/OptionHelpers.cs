using System;
using System.Collections.Generic;
using System.Text;

namespace RndWallpaper
{
	public static class OptionHelpers
	{
		public static IEnumerable<T> EnumAll<T>(bool includeZero = false)
			where T : struct, Enum
		{
			foreach(T a in Enum.GetValues<T>()) {
				int v = (int)((object)a);
				if (!includeZero && v == 0) { continue; }
				yield return a;
			};
		}

		public static void PrintEnum<T>(this StringBuilder sb, int level = 0, Func<T,string> descriptionMap = null,
			Func<T,string> nameMap = null, bool includeZero = false) where T : struct, Enum
		{
			var allEnums = new List<T>(EnumAll<T>(includeZero));
			var list = new List<(int,string,string)>();
			foreach(T e in allEnums) {
				int inum = (int)((object)e);
				string pname = nameMap == null ? e.ToString() : nameMap(e);
				string pdesc = descriptionMap == null ? "" : descriptionMap(e);
				list.Add((inum,pname,pdesc));
			}
			sb.PrintListWithBullets(list,level);
		}

		public static void PrintListWithBullets(this StringBuilder sb, IList<(int,string,string)> items, int level = 0)
		{
			int indexMax = int.MinValue;
			for(int i=0; i<items.Count; i++) {
				if (items[i].Item1 > indexMax) {
					indexMax = items[i].Item1;
				}
			}

			int numDigits = 1 + (int)Math.Floor(Math.Log10(indexMax));
			for(int i=0; i<items.Count; i++) {
				string pnum = items[i].Item1.ToString();
				int lpad = pnum.Length < numDigits ? numDigits - pnum.Length : 0;
				string npad = new string(' ',lpad);
				string pname = items[i].Item2 ?? "";
				string pdsc = items[i].Item3 ?? "";
				sb.WL(level,$"{npad}{pnum}. {pname}",pdsc);
			}
		}

		public static void PrintMonitors(this StringBuilder sb, int level = 0)
		{
			var all = Screen.AllScreens;
			var list = new List<(int,string,string)>();

			for(int i=0; i<all.Length; i++) {
				var s = all[i];
				string name = $"{s.DeviceName} {(s.Primary ? "(Primary)" : "")}";
				string desc = $"Depth:{s.BitsPerPixel} Size:{s.Bounds}";
				list.Add((i+1,name,desc));
			}
			sb.WL(level,"Primary","Select the primary monitor");
			sb.PrintListWithBullets(list,level);
		}

		const int ColumnOffset = 30;
		public static StringBuilder WL(this StringBuilder sb, int level, string def, object desc)
		{
			int pad = level;
			return sb
				.Append(' ',pad)
				.Append(def)
				.Append(' ',ColumnOffset - def.Length - pad)
				.AppendWrap(ColumnOffset,desc.ToString());
		}

		public static StringBuilder WL(this StringBuilder sb, int level, string def)
		{
			int pad = level;
			return sb
				.Append(' ',pad)
				.AppendWrap(pad,def);
		}

		public static StringBuilder WL(this StringBuilder sb, string s = null)
		{
			return s == null ? sb.AppendLine() : sb.AppendWrap(0,s);
		}

		public static StringBuilder AppendWrap(this StringBuilder self, int offset, string m)
		{
			int w = Console.IsOutputRedirected
				? int.MaxValue
				: Console.BufferWidth - 1 - offset
			;
			int c = 0;
			int l = m.Length;

			while(c < l) {
				//need spacing after first line
				string o = c > 0 ? new string(' ',offset) : "";
				//this is the last iteration
				if (c + w >= l) {
					string s = m.Substring(c);
					c += w;
					self.Append(o).AppendLine(s);
				}
				//we're in the middle
				else {
					string s = m.Substring(c,w);
					c += w;
					self.Append(o).AppendLine(s);
				}
			}

			//we always want a newline even if m is emptys
			if (l < 1) {
				self.AppendLine();
			}

			//StringBuilder likes to chain
			return self;
		}

		public static bool TryParseMonitor(string val, out PickMonitor index)
		{
			index = PickMonitor.All;
			if (val.Equals("primary",StringComparison.InvariantCultureIgnoreCase)) {
				index = PickMonitor.Primary;
				return true;
			}

			if (int.TryParse(val,out int rawIndex)) {
				index = (PickMonitor)rawIndex;
				return true;
			}

			return false;
		}

		public static bool TryParseEnumFirstLetter<T>(string arg, out T val) where T : struct, Enum
		{
			bool worked = Enum.TryParse<T>(arg,true,out val);
			//try to match the first letter if normal enum parse fails
			if (!worked) {
				string f = arg.Substring(0,1);
				foreach(T e in Enum.GetValues<T>()) {
					string name = e.ToString();
					if (name.Equals("none",StringComparison.OrdinalIgnoreCase)) {
						continue;
					}
					string n = name.Substring(0,1);
					if (f.Equals(n,StringComparison.OrdinalIgnoreCase)) {
						val = e;
						return true;
					}
				}
			}
			return worked;
		}
	}
}