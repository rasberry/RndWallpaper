using System;
using System.ComponentModel;
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
			//return new Win32Exception(HResult).Message;
			return MessageFromHResult(HResult);
		}}

		const int MaxAllowedBufferSize = 65 * 1024;
		const int ERROR_INSUFFICIENT_BUFFER = 0x7A;
		static bool TryGetErrorMessage(uint error, StringBuilder sb, out string errorMsg)
		{
			errorMsg = "";
			var flags = Format_Message_Flags.IGNORE_INSERTS
				| Format_Message_Flags.ARGUMENT_ARRAY
				| Format_Message_Flags.FROM_SYSTEM;
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

		public static PickFacility FacilityFromHResult(uint hResult)
		{
			uint facility = hResult >> 16 & 0x07FFu;
			if (Enum.IsDefined(typeof(PickFacility),facility)) {
				return (PickFacility)facility;
			}
			return PickFacility.Unknown;
		}

	}

	public enum PickFacility : uint
	{
		Unknown                                  = 0xFFFF,
		NULL                                     = 0x0000,
		RPC                                      = 0x0001,
		DISPATCH                                 = 0x0002,
		STORAGE                                  = 0x0003,
		ITF                                      = 0x0004,
		WIN32                                    = 0x0007,
		WINDOWS                                  = 0x0008,
		SECURITY                                 = 0x0009,
		SSPI                                     = 0x0009,
		CONTROL                                  = 0x000A,
		CERT                                     = 0x000B,
		INTERNET                                 = 0x000C,
		MEDIASERVER                              = 0x000D,
		MSMQ                                     = 0x000E,
		SETUPAPI                                 = 0x000F,
		SCARD                                    = 0x0010,
		COMPLUS                                  = 0x0011,
		AAF                                      = 0x0012,
		URT                                      = 0x0013,
		ACS                                      = 0x0014,
		DPLAY                                    = 0x0015,
		UMI                                      = 0x0016,
		SXS                                      = 0x0017,
		WINDOWS_CE                               = 0x0018,
		HTTP                                     = 0x0019,
		USERMODE_COMMONLOG                       = 0x001A,
		WER                                      = 0x001B,
		USERMODE_FILTER_MANAGER                  = 0x001F,
		BACKGROUNDCOPY                           = 0x0020,
		CONFIGURATION                            = 0x0021,
		WIA                                      = 0x0021,
		STATE_MANAGEMENT                         = 0x0022,
		METADIRECTORY                            = 0x0023,
		WINDOWSUPDATE                            = 0x0024,
		DIRECTORYSERVICE                         = 0x0025,
		GRAPHICS                                 = 0x0026,
		NAP                                      = 0x0027,
		SHELL                                    = 0x0027,
		TPM_SERVICES                             = 0x0028,
		TPM_SOFTWARE                             = 0x0029,
		UI                                       = 0x002A,
		XAML                                     = 0x002B,
		ACTION_QUEUE                             = 0x002C,
		PLA                                      = 0x0030,
		WINDOWS_SETUP                            = 0x0030,
		FVE                                      = 0x0031,
		FWP                                      = 0x0032,
		WINRM                                    = 0x0033,
		NDIS                                     = 0x0034,
		USERMODE_HYPERVISOR                      = 0x0035,
		CMI                                      = 0x0036,
		USERMODE_VIRTUALIZATION                  = 0x0037,
		USERMODE_VOLMGR                          = 0x0038,
		BCD                                      = 0x0039,
		USERMODE_VHD                             = 0x003A,
		SDIAG                                    = 0x003C,
		WEBSERVICES                              = 0x003D,
		WINPE                                    = 0x003D,
		WPN                                      = 0x003E,
		WINDOWS_STORE                            = 0x003F,
		INPUT                                    = 0x0040,
		EAP                                      = 0x0042,
		WINDOWS_DEFENDER                         = 0x0050,
		OPC                                      = 0x0051,
		XPS                                      = 0x0052,
		RAS                                      = 0x0053,
		MBN                                      = 0x0054,
		POWERSHELL                               = 0x0054,
		EAS                                      = 0x0055,
		P2P_INT                                  = 0x0062,
		P2P                                      = 0x0063,
		DAF                                      = 0x0064,
		BLUETOOTH_ATT                            = 0x0065,
		AUDIO                                    = 0x0066,
		VISUALCPP                                = 0x006D,
		SCRIPT                                   = 0x0070,
		PARSE                                    = 0x0071,
		BLB                                      = 0x0078,
		BLB_CLI                                  = 0x0079,
		WSBAPP                                   = 0x007A,
		BLBUI                                    = 0x0080,
		USN                                      = 0x0081,
		USERMODE_VOLSNAP                         = 0x0082,
		TIERING                                  = 0x0083,
		WSB_ONLINE                               = 0x0085,
		ONLINE_ID                                = 0x0086,
		DLS                                      = 0x0099,
		SOS                                      = 0x00A0,
		DEBUGGERS                                = 0x00B0,
		USERMODE_SPACES                          = 0x00E7,
		DMSERVER                                 = 0x0100,
		RESTORE                                  = 0x0100,
		SPP                                      = 0x0100,
		DEPLOYMENT_SERVICES_SERVER               = 0x0101,
		DEPLOYMENT_SERVICES_IMAGING              = 0x0102,
		DEPLOYMENT_SERVICES_MANAGEMENT           = 0x0103,
		DEPLOYMENT_SERVICES_UTIL                 = 0x0104,
		DEPLOYMENT_SERVICES_BINLSVC              = 0x0105,
		DEPLOYMENT_SERVICES_PXE                  = 0x0107,
		DEPLOYMENT_SERVICES_TFTP                 = 0x0108,
		DEPLOYMENT_SERVICES_TRANSPORT_MANAGEMENT = 0x0110,
		DEPLOYMENT_SERVICES_DRIVER_PROVISIONING  = 0x0116,
		DEPLOYMENT_SERVICES_MULTICAST_SERVER     = 0x0121,
		DEPLOYMENT_SERVICES_MULTICAST_CLIENT     = 0x0122,
		DEPLOYMENT_SERVICES_CONTENT_PROVIDER     = 0x0125,
		LINGUISTIC_SERVICES                      = 0x0131,
		WEB                                      = 0x0375,
		WEB_SOCKET                               = 0x0376,
		AUDIOSTREAMING                           = 0x0446,
		ACCELERATOR                              = 0x0600,
		MOBILE                                   = 0x0701,
		WMAAECMA                                 = 0x07CC,
		WEP                                      = 0x0801,
		SYNCENGINE                               = 0x0802,
		DIRECTMUSIC                              = 0x0878,
		DIRECT3D10                               = 0x0879,
		DXGI                                     = 0x087A,
		DXGI_DDI                                 = 0x087B,
		DIRECT3D11                               = 0x087C,
		LEAP                                     = 0x0888,
		AUDCLNT                                  = 0x0889,
		WINCODEC_DWRITE_DWM                      = 0x0898,
		DIRECT2D                                 = 0x0899,
		DEFRAG                                   = 0x0900,
		USERMODE_SDBUS                           = 0x0901,
		JSCRIPT                                  = 0x0902,
		PIDGENX                                  = 0x0A01
	}
}
