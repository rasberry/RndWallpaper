using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace RndWallpaper.Windows;

[SupportedOSPlatform("windows")]
public sealed class DeviceWinVirtual : IDevice, IDisposable
{
	static DeviceWinVirtual()
	{
		var shell = (IServiceProvider10)Activator.CreateInstance(Type.GetTypeFromCLSID(Guids.CLSID_ImmersiveShell));
		VirtualDesktopManagerInternal = (IVirtualDesktopManagerInternal)shell.QueryService(Guids.CLSID_VirtualDesktopManagerInternal, typeof(IVirtualDesktopManagerInternal).GUID);
	}

	public IReadOnlyList<IMonitor> AllMonitors { get; } = new VirtualList();

	public string GetWallPaper(IMonitor m)
	{
		var vm = (VirtualMonitor)m;
		return vm.GetWallPaper();
	}

	public void SetStyle(PickWallpaperStyle s)
	{
		//
	}

	public void SetWallPaper(IMonitor m, string imagePath)
	{
		var vm = (VirtualMonitor)m;
		VirtualDesktopManagerInternal.SetDesktopWallpaper(vm.Desktop, (HString)imagePath);
	}

	public void SetWallPaperAll(string imagePath)
	{
		VirtualDesktopManagerInternal.UpdateWallpaperPathForAllDesktops((HString)imagePath);
	}

	public void Dispose()
	{
		foreach(var item in AllMonitors) {
			var vm = (VirtualMonitor)item;
			vm?.Dispose();
		}

		if (VirtualDesktopManagerInternal != null) {
			Marshal.ReleaseComObject(VirtualDesktopManagerInternal);
		}
	}

	internal static IVirtualDesktopManagerInternal VirtualDesktopManagerInternal;

	class VirtualList : IReadOnlyList<IMonitor>
	{
		public VirtualList()
		{
			//initialize collection so we can populate just-in-time
			int count = Count;
			for(int i = 0; i < count; i++) {
				Storage.Add(null);
			}
		}

		public IMonitor this[int index] { get {
			PopMonitor(index);
			return Storage[index];
		}}

		public int Count { get {
			return VirtualDesktopManagerInternal.GetCount();
		}}

		public IEnumerator<IMonitor> GetEnumerator() {
			return Storage.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		void PopMonitor(int index)
		{
			if (Storage[index] != null) { return; }

			if (index < 0 || index >= Count) {
				throw new ArgumentOutOfRangeException(nameof(index));
			}
			VirtualDesktopManagerInternal.GetDesktops(out IObjectArray desktops);
			desktops.GetAt(index, typeof(IVirtualDesktop).GUID, out object objdesktop);
			Marshal.ReleaseComObject(desktops);
			
			var monitor = new VirtualMonitor((IVirtualDesktop)objdesktop);
			Storage[index] = monitor;
		}

		readonly List<IMonitor> Storage = new();
	}

	class VirtualMonitor : IMonitor, IDisposable
	{
		public VirtualMonitor(IVirtualDesktop desk)
		{
			Desktop = desk;
			Id = desk.GetId();
			var curr = VirtualDesktopManagerInternal.GetCurrentDesktop();
			this.IsPrimary = Id == curr.GetId();
			Name = (string)desk.GetName();
		}

		public readonly IVirtualDesktop Desktop;

		public string Name { get; init; }
		public bool IsPrimary { get; init; }
		public int? BitsPerPixel { get { return null; }}
		public int? PhysicalWidthMm { get { return null; }}
		public int? PhysicalHeightMm { get { return null; }}
		public int? VerticalRefresh { get { return null; }}
		public Rectangle? Dimensions { get { return null; }}

		public Guid Id { get; init; }
		public string GetWallPaper() {
			return (string)Desktop.GetWallpaperPath();
		}

		public void Dispose()
		{
			if (Desktop != null) {
				Marshal.ReleaseComObject(Desktop);
			}
		}
	}
}