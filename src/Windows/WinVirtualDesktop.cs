using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using SixLabors.ImageSharp.ColorSpaces;

namespace RndWallpaper.Windows;

[StructLayout(LayoutKind.Sequential)]
internal struct SIZE
{
	public int X;
	public int Y;
}

internal enum APPLICATION_VIEW_CLOAK_TYPE : int
{
	AVCT_NONE = 0,
	AVCT_DEFAULT = 1,
	AVCT_VIRTUAL_DESKTOP = 2
}

internal enum APPLICATION_VIEW_COMPATIBILITY_POLICY : int
{
	AVCP_NONE = 0,
	AVCP_SMALL_SCREEN = 1,
	AVCP_TABLET_SMALL_SCREEN = 2,
	AVCP_VERY_SMALL_SCREEN = 3,
	AVCP_HIGH_SCALE_FACTOR = 4
}

internal static class Guids
{
	public static readonly Guid CLSID_ImmersiveShell = new Guid("C2F03A33-21F5-47FA-B4BB-156362A2F239");
	public static readonly Guid CLSID_VirtualDesktopManagerInternal = new Guid("C5E0CDCA-7B6E-41B2-9FC4-D93975CC467B");
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("92CA9DCD-5622-4BBA-A805-5E9F541BD8C9")]
internal interface IObjectArray
{
	void GetCount(out int count);
	void GetAt(int index, ref Guid iid, [MarshalAs(UnmanagedType.Interface)]out object obj);
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("53F5CA0B-158F-4124-900C-057158060B27")]
internal interface IVirtualDesktopManagerInternal
{
	int GetCount();
	void MoveViewToDesktop(IApplicationView view, IVirtualDesktop desktop);
	bool CanViewMoveDesktops(IApplicationView view);
	IVirtualDesktop GetCurrentDesktop();
	void GetDesktops(out IObjectArray desktops);
	[PreserveSig]
	int GetAdjacentDesktop(IVirtualDesktop from, int direction, out IVirtualDesktop desktop);
	void SwitchDesktop(IVirtualDesktop desktop);
	void SwitchDesktopAndMoveForegroundView(IVirtualDesktop desktop);
	IVirtualDesktop CreateDesktop();
	void MoveDesktop(IVirtualDesktop desktop, int nIndex);
	void RemoveDesktop(IVirtualDesktop desktop, IVirtualDesktop fallback);
	IVirtualDesktop FindDesktop(ref Guid desktopid);
	void GetDesktopSwitchIncludeExcludeViews(IVirtualDesktop desktop, out IObjectArray unknown1, out IObjectArray unknown2);
	void SetDesktopName(IVirtualDesktop desktop, HString name);
	void SetDesktopWallpaper(IVirtualDesktop desktop, HString path);
	void UpdateWallpaperPathForAllDesktops(HString path);
	void CopyDesktopState(IApplicationView pView0, IApplicationView pView1);
	void CreateRemoteDesktop(HString path, out IVirtualDesktop desktop);
	void SwitchRemoteDesktop(IVirtualDesktop desktop, IntPtr switchtype);
	void SwitchDesktopWithAnimation(IVirtualDesktop desktop);
	void GetLastActiveDesktop(out IVirtualDesktop desktop);
	void WaitForAnimationToComplete();
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
[Guid("372E1D3B-38D3-42E4-A15B-8AB2B178F513")]
internal interface IApplicationView
{
	int SetFocus();
	int SwitchTo();
	int TryInvokeBack(IntPtr /* IAsyncCallback* */ callback);
	int GetThumbnailWindow(out IntPtr hwnd);
	int GetMonitor(out IntPtr /* IImmersiveMonitor */ immersiveMonitor);
	int GetVisibility(out int visibility);
	int SetCloak(APPLICATION_VIEW_CLOAK_TYPE cloakType, int unknown);
	int GetPosition(ref Guid guid /* GUID for IApplicationViewPosition */, out IntPtr /* IApplicationViewPosition** */ position);
	int SetPosition(ref IntPtr /* IApplicationViewPosition* */ position);
	int InsertAfterWindow(IntPtr hwnd);
	int GetExtendedFramePosition(out RECT rect);
	int GetAppUserModelId([MarshalAs(UnmanagedType.LPWStr)] out string id);
	int SetAppUserModelId(string id);
	int IsEqualByAppUserModelId(string id, out int result);
	int GetViewState(out uint state);
	int SetViewState(uint state);
	int GetNeediness(out int neediness);
	int GetLastActivationTimestamp(out ulong timestamp);
	int SetLastActivationTimestamp(ulong timestamp);
	int GetVirtualDesktopId(out Guid guid);
	int SetVirtualDesktopId(ref Guid guid);
	int GetShowInSwitchers(out int flag);
	int SetShowInSwitchers(int flag);
	int GetScaleFactor(out int factor);
	int CanReceiveInput(out bool canReceiveInput);
	int GetCompatibilityPolicyType(out APPLICATION_VIEW_COMPATIBILITY_POLICY flags);
	int SetCompatibilityPolicyType(APPLICATION_VIEW_COMPATIBILITY_POLICY flags);
	int GetSizeConstraints(IntPtr /* IImmersiveMonitor* */ monitor, out SIZE size1, out SIZE size2);
	int GetSizeConstraintsForDpi(uint uint1, out SIZE size1, out SIZE size2);
	int SetSizeConstraintsForDpi(ref uint uint1, ref SIZE size1, ref SIZE size2);
	int OnMinSizePreferencesUpdated(IntPtr hwnd);
	int ApplyOperation(IntPtr /* IApplicationViewOperation* */ operation);
	int IsTray(out bool isTray);
	int IsInHighZOrderBand(out bool isInHighZOrderBand);
	int IsSplashScreenPresented(out bool isSplashScreenPresented);
	int Flash();
	int GetRootSwitchableOwner(out IApplicationView rootSwitchableOwner);
	int EnumerateOwnershipTree(out IObjectArray ownershipTree);
	int GetEnterpriseId([MarshalAs(UnmanagedType.LPWStr)] out string enterpriseId);
	int IsMirrored(out bool isMirrored);
	int Unknown1(out int unknown);
	int Unknown2(out int unknown);
	int Unknown3(out int unknown);
	int Unknown4(out int unknown);
	int Unknown5(out int unknown);
	int Unknown6(int unknown);
	int Unknown7();
	int Unknown8(out int unknown);
	int Unknown9(int unknown);
	int Unknown10(int unknownX, int unknownY);
	int Unknown11(int unknown);
	int Unknown12(out SIZE size1);
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("3F07F4BE-B107-441A-AF0F-39D82529072C")]
internal interface IVirtualDesktop
{
	bool IsViewVisible(IApplicationView view);
	Guid GetId();
	HString GetName();
	HString GetWallpaperPath();
	bool IsRemote();
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
internal interface IServiceProvider10
{
	[return: MarshalAs(UnmanagedType.IUnknown)]
	object QueryService(ref Guid service, ref Guid riid);
}

// .NET 5 has removed WinRT support, so HString cannot marshal to System.String.
// Since marshalling with UnmanagedType.HString fails, use IntPtr to get the string via C#/WinRT MarshalString.
// see also: https://github.com/microsoft/CsWinRT/blob/master/docs/interop.md
[SupportedOSPlatform("windows")]
[StructLayout(LayoutKind.Sequential)]
public readonly struct HString
{
	readonly IntPtr _abi;

	internal HString(string str)
	{
		this._abi = WinRT.MarshalString.GetAbi(WinRT.MarshalString.CreateMarshaler(str));
	}
	
	public static explicit operator string(HString hStr)
	{
		return WinRT.MarshalString.FromAbi(hStr._abi);
	}
	public static explicit operator HString(string str)
	{
		return new HString(str);
	}
}

