using System;
using System.Collections.Generic;

namespace RndWallpaper
{
	public sealed class Params
	{
		public enum Result {
			Missing = 0,
			Invalid = 1,
			Good = 2
		}

		//in case you might need to include a custom parser
		public delegate bool Parser<T>(string inp, out T val);

		public Params(string[] args)
		{
			Args = new List<string>(args);
		}

		List<string> Args;

		public string[] Remaining()
		{
			return Args.ToArray();
		}

		// check for existance of a single parameter
		public Result Has(params string[] @switch)
		{
			int ii = -1;
			foreach(string sw in @switch) {
				int i = Args.IndexOf(sw);
				if (i != -1) {
					Args.RemoveAt(i);
					ii = i;
				}
			}
			return ii == -1 ? Result.Missing : Result.Good;
		}

		// check for a non-qualified (leftover) parameter
		public Result Default<T>(out T val, T def = default(T),Parser<T> par = null)
		{
			val = def;
			if (Args.Count <= 0) { return Result.Missing; }
			string curr = Args[0];
			if (par == null) { par = TryParse; }
			if (!par(curr,out val)) {
				return Result.Invalid;
			}
			Args.RemoveAt(0);
			return Result.Good;
		}

		//find or default a parameter with one argument
		public Result Default<T>(string @switch,out T val,T def = default(T),Parser<T> par = null,
			bool logMessages = true)
		{
			val = def;
			int i = Args.IndexOf(@switch);
			if (i == -1) {
				return Result.Missing;
			}
			if (i+1 >= Args.Count) {
				if (logMessages) { Log.MissingArgument(@switch); }
				return Result.Invalid;
			}
			if (par == null) { par = TryParse; }
			bool worked = par(Args[i+1],out val);
			if (!par(Args[i+1],out val)) {
				if (logMessages) { Log.CouldNotParse(@switch,Args[i+1]); }
				return Result.Invalid;
			}
			Args.RemoveAt(i+1);
			Args.RemoveAt(i);
			return Result.Good;
		}

		public Result Default<T>(string[] @switch,out T val,T def = default(T),Parser<T> par = null)
		{
			val = default(T);
			Result rr = Result.Missing;
			foreach(string sw in @switch) {
				var r = Default<T>(sw,out val,def,par);
				// Log.Debug($"Default sw={sw} r={r} val={val}");
				if (r == Result.Invalid) { return r; }
				if (r == Result.Good) { return r; }
			}
			return rr;
		}

		//find or default a parameter with two arguments
		//Condition function determines when second argument is required (defaults to always true)
		public Result Default<T,U>(string @switch,out T tval, out U uval,
			T tdef = default(T), U udef = default(U), Func<T,bool> Cond = null,
			Parser<T> tpar = null, Parser<U> upar = null)
		{
			tval = tdef;
			uval = udef;
			int i = Args.IndexOf(@switch);
			if (i == -1) {
				return Result.Missing;
			}
			if (i+1 >= Args.Count) {
				Log.MissingArgument(@switch);
				return Result.Invalid;
			}
			if (tpar == null) { tpar = TryParse; }
			if (!tpar(Args[i+1],out tval)) {
				Log.CouldNotParse(@switch,Args[i+1]);
				return Result.Invalid;
			}

			//if condition function returns false - we don't look for a second arg
			if (Cond != null && !Cond(tval)) {
				Args.RemoveAt(i+1);
				Args.RemoveAt(i);
				return Result.Good;
			}

			if (i+2 >= Args.Count) {
				Log.MissingArgument(@switch);
				return Result.Invalid;
			}
			if (upar == null) { upar = TryParse; }
			if (!upar(Args[i+2],out uval)) {
				Log.CouldNotParse(@switch,Args[i+2]);
				return Result.Invalid;
			}
			Args.RemoveAt(i+2);
			Args.RemoveAt(i+1);
			Args.RemoveAt(i);
			return Result.Good;
		}

		public Result Expect<T>(out T val, string name)
		{
			if (Result.Good != Default(out val)) {
				Log.MustProvideInput(name);
				return Result.Invalid;
			}
			return Result.Good;
		}

		public Result Expect<T>(out T val, string name, Parser<T> par = null)
		{
			if (Result.Good != Default(out val,par:par)) {
				Log.MustProvideInput(name);
				return Result.Invalid;
			}
			return Result.Good;
		}

		public Result Expect(string @switch)
		{
			var has = Has(@switch);
			if (Result.Good != has) {
				if (has == Result.Missing) {
					Log.MustProvideInput(@switch);
				}
				return Result.Invalid;
			}
			return Result.Good;
		}

		public Result Expect<T>(string @switch, out T val,Parser<T> par = null)
		{
			var has = Default(@switch,out val, par:par);
			if (Result.Good != has) {
				if (has == Result.Missing) {
					Log.MustProvideInput(@switch);
				}
				return Result.Invalid;
			}
			return Result.Good;
		}

		public Result Expect<T,U>(string @switch, out T tval, out U uval,Parser<T> tpar = null,Parser<U> upar = null)
		{
			var has = Default(@switch,out tval,out uval, tpar:tpar, upar:upar);
			if (Result.Good != has) {
				if (has == Result.Missing) {
					Log.MustProvideInput(@switch);
				}
				return Result.Invalid;
			}
			return Result.Good;
		}

		// consolidated the tryparse here - trying to make the code a bit more portable
		public static bool TryParse<V>(string sub, out V val)
		{
			val = default(V);
			Type t = typeof(V);

			var nullType = Nullable.GetUnderlyingType(t);
			if (nullType != null) { t = nullType; }

			if (t.IsEnum) {
				if (Enum.TryParse(t,sub,true,out object o)) {
					val = (V)o;
					return Enum.IsDefined(t,o);
				}
			}
			else if (t.Equals(typeof(double))) {
				if (double.TryParse(sub,out double b)) {
					if (!double.IsInfinity(b) && !double.IsNaN(b)) {
						val = (V)((object)b);
						return true;
					}
				}
			}
			else if (t.Equals(typeof(int))) {
				if (int.TryParse(sub,out int b)) {
					val = (V)((object)b); return true;
				}
			}
			else if (t.Equals(typeof(string))) {
				if (!String.IsNullOrWhiteSpace(sub)) {
					val = (V)((object)sub);
					return true;
				}
			}
			/*
			else if (t.Equals(typeof(Color))) {
				if (TryParseColor(sub,out var clr)) {
					val = (V)((object)clr); return true;
				}
			}
			*/
			return false;
		}
	}

	public static class ParamsExtensions
	{
		public static bool IsGood(this Params.Result r)
		{
			return r == Params.Result.Good;
		}

		public static bool IsBad(this Params.Result r)
		{
			return r != Params.Result.Good;
		}

		public static bool IsInvalid(this Params.Result r)
		{
			return r == Params.Result.Invalid;
		}

		public static bool IsMissing(this Params.Result r)
		{
			return r == Params.Result.Missing;
		}

	}
}