using System;
using System.Runtime.InteropServices;

// https://github.com/xupefei/Bing-Wallpaper/blob/master/BingWallpaper/DesktopWallpaper.cs
namespace RndWallpaper
{
	/// <summary>
	/// This enumeration is used to set and get slideshow options.
	/// </summary>
	public enum DesktopSlideshowOptions
	{
		ShuffleImages = 0x01,
		// When set, indicates that the order in which images in the slideshow are displayed can be randomized.
	}

	/// <summary>
	/// This enumeration is used by GetStatus to indicate the current status of the slideshow.
	/// </summary>
	public enum DesktopSlideshowState
	{
		Enabled = 0x01,
		Slideshow = 0x02,
		DisabledByRemoteSession = 0x04,
	}

	/// <summary>
	/// This enumeration is used by the AdvanceSlideshow method to indicate whether to advance the slideshow forward or
	/// backward.
	/// </summary>
	public enum DesktopSlideshowDirection
	{
		Forward = 0,
		Backward = 1,
	}

	/// <summary>
	/// This enumeration indicates the wallpaper position for all monitors. (This includes when slideshows are running.)
	/// The wallpaper position specifies how the image that is assigned to a monitor should be displayed.
	/// </summary>
	public enum DesktopWallpaperPosition
	{
		Center = 0,
		Tile = 1,
		Stretch = 2,
		Fit = 3,
		Fill = 4,
		Span = 5,
	}

	#if false //keep this in case IDesktopWallpaperPrivate goes away
	[ComImport, Guid("B92B56A9-8B55-4E14-9A89-0199BBB6F93B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IDesktopWallpaper
	{
		void SetWallpaper(
			[MarshalAs(UnmanagedType.LPWStr)] string monitorID,
			[MarshalAs(UnmanagedType.LPWStr)] string wallpaper
		);

		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorID);

		/// <summary>
		///     Gets the monitor device path.
		/// </summary>
		/// <param name="monitorIndex">Index of the monitor device in the monitor device list.</param>
		/// <returns></returns>
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetMonitorDevicePathAt(uint monitorIndex);

		/// <summary>
		///     Gets number of monitor device paths.
		/// </summary>
		/// <returns></returns>
		[return: MarshalAs(UnmanagedType.U4)]
		uint GetMonitorDevicePathCount();

		[return: MarshalAs(UnmanagedType.Struct)]
		RECT GetMonitorRECT([MarshalAs(UnmanagedType.LPWStr)] string monitorID);

		void SetBackgroundColor([MarshalAs(UnmanagedType.U4)] uint color);

		[return: MarshalAs(UnmanagedType.U4)]
		uint GetBackgroundColor();

		void SetPosition([MarshalAs(UnmanagedType.I4)] DesktopWallpaperPosition position);

		[return: MarshalAs(UnmanagedType.I4)]
		DesktopWallpaperPosition GetPosition();

		void SetSlideshow(IntPtr items);
		IntPtr GetSlideshow();

		void SetSlideshowOptions(DesktopSlideshowDirection options, uint slideshowTick);

		[PreserveSig]
		uint GetSlideshowOptions(out DesktopSlideshowDirection options, out uint slideshowTick);

		void AdvanceSlideshow(
			[MarshalAs(UnmanagedType.LPWStr)] string monitorID,
			[MarshalAs(UnmanagedType.I4)] DesktopSlideshowDirection direction
		);

		DesktopSlideshowDirection GetStatus();

		bool Enable();
	}
	#endif

	#if falase
	[ComImport, Guid("C182461F-DFAC-4375-AB6E-4CC45AA7F9CC"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IDesktopWallpaperPrivate
	{
		//== IDesktopWallpaper Methods

		void SetWallpaper(
			[MarshalAs(UnmanagedType.LPWStr)] string monitorID,
			[MarshalAs(UnmanagedType.LPWStr)] string wallpaper
		);

		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorID);

		/// <summary>
		///     Gets the monitor device path.
		/// </summary>
		/// <param name="monitorIndex">Index of the monitor device in the monitor device list.</param>
		/// <returns></returns>
		[return: MarshalAs(UnmanagedType.LPWStr)]
		string GetMonitorDevicePathAt(uint monitorIndex);

		/// <summary>
		///     Gets number of monitor device paths.
		/// </summary>
		/// <returns></returns>
		[return: MarshalAs(UnmanagedType.U4)]
		uint GetMonitorDevicePathCount();

		[return: MarshalAs(UnmanagedType.Struct)]
		RECT GetMonitorRECT([MarshalAs(UnmanagedType.LPWStr)] string monitorID);

		void SetBackgroundColor([MarshalAs(UnmanagedType.U4)] uint color);

		[return: MarshalAs(UnmanagedType.U4)]
		uint GetBackgroundColor();

		void SetPosition([MarshalAs(UnmanagedType.I4)] DesktopWallpaperPosition position);

		[return: MarshalAs(UnmanagedType.I4)]
		DesktopWallpaperPosition GetPosition();

		void SetSlideshow(IntPtr items);
		IntPtr GetSlideshow();

		void SetSlideshowOptions(DesktopSlideshowDirection options, uint slideshowTick);

		[PreserveSig]
		uint GetSlideshowOptions(out DesktopSlideshowDirection options, out uint slideshowTick);

		void AdvanceSlideshow(
			[MarshalAs(UnmanagedType.LPWStr)] string monitorID,
			[MarshalAs(UnmanagedType.I4)] DesktopSlideshowDirection direction
		);

		DesktopSlideshowDirection GetStatus();

		bool Enable();

		//== IDesktopWallpaperPrivate Methods
		// https://github.com/alur/litestep-experimental/blob/master/litestep/DesktopWallpaper.h

		void SetWallpaper2(
			[MarshalAs(UnmanagedType.LPWStr)] string monitorID,
			[MarshalAs(UnmanagedType.LPWStr)] string wallpaper
		);

		void Proc20(IntPtr p0, int p1, int p2);

		[return: MarshalAs(UnmanagedType.I4)]
		int GetWallpaperColor();

		[return: MarshalAs(UnmanagedType.I4)]
		int GetMonitorNumber([MarshalAs(UnmanagedType.LPWStr)] string monitorID);

		void Proc23();
		void Proc24();
	}

	#endif

	#if false
	//TODO not working with native compilation 6.0.0-alpha.*
	// try https://github.com/dotnet/corert/issues/6252#issuecomment-415591702
	/// <summary>
	/// CoClass DesktopWallpaper
	/// </summary>
	[ComImport, Guid("C2CF3110-460E-4fc1-B9D0-8A1C0C9CC4BD")]
	public class DesktopWallpaperClass { }
	#endif

	// Used example from corert team to implement manual interop
	// so that I can still use native compilation
	// https://github.com/dotnet/corert/issues/6252#issuecomment-415591702
	public sealed class DesktopWallpaperClass : IDisposable
	{
		public DesktopWallpaperClass()
		{
			Init();
		}

		void Init()
		{
			int hr = WinMethods.CoInitializeEx(IntPtr.Zero, COINIT.COINIT_APARTMENTTHREADED);
			Marshal.ThrowExceptionForHR(hr, new IntPtr(-1));

			// DesktopWallpaper Class
			Guid ClassId = new Guid("C2CF3110-460E-4fc1-B9D0-8A1C0C9CC4BD");
			// IDesktopWallPaper
			// Guid InterfaceId = new Guid("B92B56A9-8B55-4E14-9A89-0199BBB6F93B");
			// IDesktopWallPaperPrivate
			Guid InterfaceId = new Guid("C182461F-DFAC-4375-AB6E-4CC45AA7F9CC");
			IntPtr ppv;

			int hResult = WinMethods.CoCreateInstance(
				ref ClassId, IntPtr.Zero, CLSCTX.CLSCTX_LOCAL_SERVER, ref InterfaceId, out ppv);

			Marshal.ThrowExceptionForHR(hResult, new IntPtr(-1));
			Pointer = ppv;
		}

		//from oleviewdotnet - https://github.com/tyranid/oleviewdotnet
		enum ProcOffset : int
		{
			IUnknown_QueryInterface = 0,                     /* [In] IntPtr pthis, [In] IntPtr priid, [Out] IntPtr ppvObject */
			IUnknown_AddRef = 1,                             /* [In] IntPtr pthis */
			IUnknown_Release = 2,                            /* [In] IntPtr pthis */
			IDesktopWallpaper_SetWallpaper = 3,              /* [In] wchar_t* p0, [In] wchar_t* p1 */
			IDesktopWallpaper_GetWallpaper = 4,              /* [In] wchar_t* p0, [Out] wchar_t** p1 */
			IDesktopWallpaper_GetMonitorDevicePathAt = 5,    /* [In] int p0, [Out] wchar_t** p1 */
			IDesktopWallpaper_GetMonitorDevicePathCount = 6, /* [Out] int* p0 */
			IDesktopWallpaper_GetMonitorRECT = 7,            /* [In] wchar_t* p0, [Out] struct RECT* p1 */
			IDesktopWallpaper_SetBackgroundColor = 8,        /* [In] int p0 */
			IDesktopWallpaper_GetBackgroundColor = 9,        /* [Out] int* p0 */
			IDesktopWallpaper_SetPosition = 10,              /* [In] int p0 */
			IDesktopWallpaper_GetPosition = 11,              /* [Out] int* p0 */
			IDesktopWallpaper_SetSlideshow = 12,             /* [In] IShellItemArray* p0 */
			IDesktopWallpaper_GetSlideshow = 13,             /* [Out] IShellItemArray** p0 */
			IDesktopWallpaper_SetSlideshowOptions = 14,      /* [In] int p0, [In] int p1 */
			IDesktopWallpaper_GetSlideshowOptions = 15,      /* [Out] int* p0, [Out] int* p1 */
			IDesktopWallpaper_AdvanceSlideshow = 16,         /* [In] wchar_t* p0, [In] int p1 */
			IDesktopWallpaper_GetStatus = 17,                /* [Out] int* p0 */
			IDesktopWallpaper_Enable = 18,                   /* [In] int p0 */
			IDesktopWallpaperPrivate_SetWallpaper2 = 19,     /* [In] wchar_t* p0, [In] wchar_t* p1 */
			IDesktopWallpaperPrivate_Proc20 = 20,            /* [In] IntPtr p0, [In] int p1, [In] int p2 */
			IDesktopWallpaperPrivate_GetWallpaperColor = 21, /* [Out] int* p0 */
			IDesktopWallpaperPrivate_GetMonitorNumber = 22,  /* [In] wchar_t* p0, [Out] int* p1 */
			IDesktopWallpaperPrivate_Proc23 = 23,            /* */
			IDesktopWallpaperPrivate_Proc24 = 24             /* */
		}

		delegate int IUnknown_Release(IntPtr thisPtr);
		void Release(IntPtr thisPtr)
		{
			var vp = GetComOffset(Pointer,ProcOffset.IUnknown_Release);
			var func = Marshal.GetDelegateForFunctionPointer<IUnknown_Release>(vp);
			func(Pointer);
		}

		delegate int IDesktopWallpaper_SetWallpaper(IntPtr thisPtr,
			[MarshalAs(UnmanagedType.LPWStr)] string monitorID,
			[MarshalAs(UnmanagedType.LPWStr)] string wallpaper
		);
		public void SetWallpaper(string monitorID, string wallpaper)
		{
			var vp = GetComOffset(Pointer,ProcOffset.IDesktopWallpaper_SetWallpaper);
			var func = Marshal.GetDelegateForFunctionPointer<IDesktopWallpaper_SetWallpaper>(vp);
			int hr = func(Pointer,monitorID,wallpaper);
			Marshal.ThrowExceptionForHR(hr,new IntPtr(-1));
		}

		delegate int IDesktopWallpaper_GetMonitorDevicePathAt(IntPtr thisPtr,
			[MarshalAs(UnmanagedType.U4)] uint index,
			[MarshalAs(UnmanagedType.LPWStr)] out string monitorId
		);
		public string GetMonitorDevicePathAt(uint index)
		{
			var vp = GetComOffset(Pointer,ProcOffset.IDesktopWallpaper_GetMonitorDevicePathAt);
			var func = Marshal.GetDelegateForFunctionPointer<IDesktopWallpaper_GetMonitorDevicePathAt>(vp);
			int hr = func(Pointer,index, out string monitorId);
			Marshal.ThrowExceptionForHR(hr,new IntPtr(-1));
			return monitorId;
		}

		delegate int IDesktopWallpaper_GetMonitorDevicePathCount(IntPtr thisPtr,
			[MarshalAs(UnmanagedType.U4)] out uint count
		);
		public uint GetMonitorDevicePathCount()
		{
			var vp = GetComOffset(Pointer,ProcOffset.IDesktopWallpaper_GetMonitorDevicePathCount);
			var func = Marshal.GetDelegateForFunctionPointer<IDesktopWallpaper_GetMonitorDevicePathCount>(vp);
			int hr = func(Pointer,out uint count);
			Marshal.ThrowExceptionForHR(hr,new IntPtr(-1));
			return count;
		}

		delegate int IDesktopWallpaper_SetPosition(IntPtr thisPtr,
			[MarshalAs(UnmanagedType.I4)] int position
		);
		public void SetPosition(DesktopWallpaperPosition position)
		{
			var vp = GetComOffset(Pointer,ProcOffset.IDesktopWallpaper_SetPosition);
			var func = Marshal.GetDelegateForFunctionPointer<IDesktopWallpaper_SetPosition>(vp);
			int hr = func(Pointer,(int)position);
			Marshal.ThrowExceptionForHR(hr,new IntPtr(-1));
		}

		delegate int IDesktopWallpaperPrivate_GetMonitorNumber(IntPtr thisPtr,
			[MarshalAs(UnmanagedType.LPWStr)] string monitorID,
			[MarshalAs(UnmanagedType.I4)] out int monitorNum
		);
		public int GetMonitorNumber(string monitorID)
		{
			var vp = GetComOffset(Pointer,ProcOffset.IDesktopWallpaperPrivate_GetMonitorNumber);
			var func = Marshal.GetDelegateForFunctionPointer<IDesktopWallpaperPrivate_GetMonitorNumber>(vp);
			int hr = func(Pointer,monitorID, out int monitorNum);
			Marshal.ThrowExceptionForHR(hr,new IntPtr(-1));
			return monitorNum;
		}

		IntPtr Pointer;
		bool disposedValue;

		//helper function to get a pointer to a COM method
		// don't know how to do this without unsafe
		static unsafe IntPtr GetComOffset(IntPtr ptr, ProcOffset offset)
		{
			return *((*(IntPtr**)ptr) + (int)offset);
		}

		void Dispose(bool disposing)
		{
			if (!disposedValue) {
				if (disposing) {
					// TODO: dispose managed state (managed objects)
				}

				//cleanup COM instance
				Release(Pointer);
				Pointer = IntPtr.Zero;

				disposedValue = true;
			}
		}

		// TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
		~DesktopWallpaperClass()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}