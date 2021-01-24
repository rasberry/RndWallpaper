using System;
using System.Runtime.InteropServices;
using System.Text;

namespace RndWallpaper
{
	// https://docs.microsoft.com/en-us/windows/win32/com/structure-of-com-error-codes
	// https://docs.microsoft.com/en-us/archive/blogs/andrew_richards/hresult-facility-by-value
	// https://referencesource.microsoft.com/#System/compmod/system/componentmodel/Win32Exception.cs
	// https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-erref/0642cb2f-2075-4469-918c-4441e69c548a
	// https://winprotocoldoc.blob.core.windows.net/productionwindowsarchives/MS-ERREF/%5bMS-ERREF%5d.pdf

	public struct WinHResult
	{
		public WinHResult(int hResult) : this((uint)hResult) {}
		public WinHResult(uint hResult)
		{
			HResult = hResult;
		}

		public readonly uint HResult;

		///<summary>returns true if the highest bit of the HResult is 0</summary>
		public bool IsSuccess { get {
			return (HResult & 0x80000000u) == 0u;
		}}

		///<summary>The facility field indicates the system service responsible for the error</summary>
		public uint Facility { get {
			return HResult >> 16 & 0x07FFu;
		}}

		///<summary>The undecorated error code</summary>
		public uint Code { get {
			return HResult & 0xFFFFu;
		}}

		public string Message { get {
			// doing this instead of return new Win32Exception(HResult).Message;
			// mostly for fun and a minor reduction in overhead
			return MessageFromHResult(HResult);
		}}

		///<summary>Gets the system message associated with an HResult</summary>
		public static string MessageFromHResult(uint hResult)
		{
			string errorMsg;
			StringBuilder sb = new StringBuilder(256);
			do {
				if (TryGetErrorMessage(hResult, sb, out errorMsg))
					return errorMsg;
				else {
					// increase the capacity of the StringBuilder by 4 times.
					sb.Capacity *= 4;
				}
			}
			while (sb.Capacity < MaxAllowedBufferSize);

			// If you come here then a size as large as 65K is also not sufficient and so we give the generic errorMsg.
			return "Unknown error (0x" + Convert.ToString(hResult, 16) + ")";
		}

		const int MaxAllowedBufferSize = 65 * 1024;
		const int ERROR_INSUFFICIENT_BUFFER = 0x7A;
		static bool TryGetErrorMessage(uint error, StringBuilder sb, out string errorMsg)
		{
			errorMsg = "";
			var flags = FormatMessageFlags.IGNORE_INSERTS
				| FormatMessageFlags.ARGUMENT_ARRAY
				| FormatMessageFlags.FROM_SYSTEM;
			uint result = WinMethods.FormatMessage(flags, IntPtr.Zero, error, 0, sb, (uint)sb.Capacity + 1, null);
			if (result != 0) {
				//remove trailing junk
				int i = sb.Length;
				while (i > 0) {
					char ch = sb[i - 1];
					if (ch > 32 && ch != '.') { break; }
					i--;
				}
				errorMsg = sb.ToString(0, i);
			}
			else if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER) {
				return false;
			}
			else {
				errorMsg ="Unknown error (0x" + Convert.ToString(error, 16) + ")";
			}

			return true;
		}

	}
}
