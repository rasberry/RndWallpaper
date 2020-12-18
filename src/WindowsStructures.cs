using System;
using System.Runtime.InteropServices;

namespace RndWallpaper
{
	// https://www.pinvoke.net/default.aspx/Structures/RECT.html
	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int Left, Top, Right, Bottom;

		public RECT(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

		public int X
		{
			get { return Left; }
			set { Right -= (Left - value); Left = value; }
		}

		public int Y
		{
			get { return Top; }
			set { Bottom -= (Top - value); Top = value; }
		}

		public int Height
		{
			get { return Bottom - Top; }
			set { Bottom = value + Top; }
		}

		public int Width
		{
			get { return Right - Left; }
			set { Right = value + Left; }
		}

		public System.Drawing.Point Location
		{
			get { return new System.Drawing.Point(Left, Top); }
			set { X = value.X; Y = value.Y; }
		}

		public System.Drawing.Size Size
		{
			get { return new System.Drawing.Size(Width, Height); }
			set { Width = value.Width; Height = value.Height; }
		}

		public static implicit operator System.Drawing.Rectangle(RECT r)
		{
			return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
		}

		public static implicit operator RECT(System.Drawing.Rectangle r)
		{
			return new RECT(r);
		}

		public static bool operator ==(RECT r1, RECT r2)
		{
			return r1.Equals(r2);
		}

		public static bool operator !=(RECT r1, RECT r2)
		{
			return !r1.Equals(r2);
		}

		public bool Equals(RECT r)
		{
			return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
		}

		public override bool Equals(object obj)
		{
			if (obj is RECT)
			return Equals((RECT)obj);
			else if (obj is System.Drawing.Rectangle)
			return Equals(new RECT((System.Drawing.Rectangle)obj));
			return false;
		}

		public override int GetHashCode()
		{
			return ((System.Drawing.Rectangle)this).GetHashCode();
		}

		public override string ToString()
		{
			return $"{{Left={Left},Top={Top},Right={Right},Bottom={Bottom}}}";
		}
	}

	[Flags]
	public enum DisplayDeviceStateFlags : int
	{
		None = 0x0,
		/// <summary>The device is part of the desktop.</summary>
		AttachedToDesktop = 0x1,
		MultiDriver = 0x2,
		/// <summary>The device is part of the desktop.</summary>
		PrimaryDevice = 0x4,
		/// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
		MirroringDriver = 0x8,
		/// <summary>The device is VGA compatible.</summary>
		VGACompatible = 0x10,
		/// <summary>The device is removable; it cannot be the primary display.</summary>
		Removable = 0x20,
		/// <summary>The device has more display modes than its output devices support.</summary>
		ModesPruned = 0x8000000,
		Remote = 0x4000000,
		Disconnect = 0x2000000
	}

	// https://docs.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-display_devicew
	[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
	public struct DisplayDevice
	{
		[MarshalAs(UnmanagedType.U4)]
		public int cb;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]
		public string DeviceName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)]
		public string DeviceString;
		[MarshalAs(UnmanagedType.U4)]
		public DisplayDeviceStateFlags StateFlags;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)]
		public string DeviceID;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)]
		public string DeviceKey;
	}

	/// <summary>
	/// The MONITORINFOEX structure contains information about a display monitor.
	/// The GetMonitorInfo function stores information into a MONITORINFOEX structure or a MONITORINFO structure.
	/// The MONITORINFOEX structure is a superset of the MONITORINFO structure. The MONITORINFOEX structure adds a string member to contain a name
	/// for the display monitor.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct MonitorInfoEx
	{
		// size of a device name string
		const int CCHDEVICENAME = 32;

		/// <summary>
		/// The size, in bytes, of the structure. Set this member to sizeof(MONITORINFOEX) (72) before calling the GetMonitorInfo function.
		/// Doing so lets the function determine the type of structure you are passing to it.
		/// </summary>
		public int Size;

		/// <summary>
		/// A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
		/// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
		/// </summary>
		public RECT Monitor;

		/// <summary>
		/// A RECT structure that specifies the work area rectangle of the display monitor that can be used by applications,
		/// expressed in virtual-screen coordinates. Windows uses this rectangle to maximize an application on the monitor.
		/// The rest of the area in rcMonitor contains system windows such as the task bar and side bars.
		/// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
		/// </summary>
		public RECT WorkArea;

		/// <summary>
		/// The attributes of the display monitor.
		///
		/// This member can be the following value:
		///   1 : MONITORINFOF_PRIMARY
		/// </summary>
		public uint Flags;

		/// <summary>
		/// A string that specifies the device name of the monitor being used. Most applications have no use for a display monitor name,
		/// and so can save some bytes by using a MONITORINFO structure.
		/// </summary>
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME )]
		public string DeviceName;

		public void Init()
		{
			this.Size = 40 + 2 * CCHDEVICENAME;
			this.DeviceName = string.Empty;
		}
	}

	public enum DeviceCap
	{
		/// <summary>
		/// Device driver version
		/// </summary>
		DRIVERVERSION = 0,
		/// <summary>
		/// Device classification
		/// </summary>
		TECHNOLOGY = 2,
		/// <summary>
		/// Horizontal size in millimeters
		/// </summary>
		HORZSIZE = 4,
		/// <summary>
		/// Vertical size in millimeters
		/// </summary>
		VERTSIZE = 6,
		/// <summary>
		/// Horizontal width in pixels
		/// </summary>
		HORZRES = 8,
		/// <summary>
		/// Vertical height in pixels
		/// </summary>
		VERTRES = 10,
		/// <summary>
		/// Number of bits per pixel
		/// </summary>
		BITSPIXEL = 12,
		/// <summary>
		/// Number of planes
		/// </summary>
		PLANES = 14,
		/// <summary>
		/// Number of brushes the device has
		/// </summary>
		NUMBRUSHES = 16,
		/// <summary>
		/// Number of pens the device has
		/// </summary>
		NUMPENS = 18,
		/// <summary>
		/// Number of markers the device has
		/// </summary>
		NUMMARKERS = 20,
		/// <summary>
		/// Number of fonts the device has
		/// </summary>
		NUMFONTS = 22,
		/// <summary>
		/// Number of colors the device supports
		/// </summary>
		NUMCOLORS = 24,
		/// <summary>
		/// Size required for device descriptor
		/// </summary>
		PDEVICESIZE = 26,
		/// <summary>
		/// Curve capabilities
		/// </summary>
		CURVECAPS = 28,
		/// <summary>
		/// Line capabilities
		/// </summary>
		LINECAPS = 30,
		/// <summary>
		/// Polygonal capabilities
		/// </summary>
		POLYGONALCAPS = 32,
		/// <summary>
		/// Text capabilities
		/// </summary>
		TEXTCAPS = 34,
		/// <summary>
		/// Clipping capabilities
		/// </summary>
		CLIPCAPS = 36,
		/// <summary>
		/// Bitblt capabilities
		/// </summary>
		RASTERCAPS = 38,
		/// <summary>
		/// Length of the X leg
		/// </summary>
		ASPECTX = 40,
		/// <summary>
		/// Length of the Y leg
		/// </summary>
		ASPECTY = 42,
		/// <summary>
		/// Length of the hypotenuse
		/// </summary>
		ASPECTXY = 44,
		/// <summary>
		/// Shading and Blending caps
		/// </summary>
		SHADEBLENDCAPS = 45,

		/// <summary>
		/// Logical pixels inch in X
		/// </summary>
		LOGPIXELSX = 88,
		/// <summary>
		/// Logical pixels inch in Y
		/// </summary>
		LOGPIXELSY = 90,

		/// <summary>
		/// Number of entries in physical palette
		/// </summary>
		SIZEPALETTE = 104,
		/// <summary>
		/// Number of reserved entries in palette
		/// </summary>
		NUMRESERVED = 106,
		/// <summary>
		/// Actual color resolution
		/// </summary>
		COLORRES = 108,

		// Printing related DeviceCaps. These replace the appropriate Escapes
		/// <summary>
		/// Physical Width in device units
		/// </summary>
		PHYSICALWIDTH = 110,
		/// <summary>
		/// Physical Height in device units
		/// </summary>
		PHYSICALHEIGHT = 111,
		/// <summary>
		/// Physical Printable Area x margin
		/// </summary>
		PHYSICALOFFSETX = 112,
		/// <summary>
		/// Physical Printable Area y margin
		/// </summary>
		PHYSICALOFFSETY = 113,
		/// <summary>
		/// Scaling factor x
		/// </summary>
		SCALINGFACTORX = 114,
		/// <summary>
		/// Scaling factor y
		/// </summary>
		SCALINGFACTORY = 115,

		/// <summary>
		/// Current vertical refresh rate of the display device (for displays only) in Hz
		/// </summary>
		VREFRESH = 116,
		/// <summary>
		/// Vertical height of entire desktop in pixels
		/// </summary>
		DESKTOPVERTRES = 117,
		/// <summary>
		/// Horizontal width of entire desktop in pixels
		/// </summary>
		DESKTOPHORZRES = 118,
		/// <summary>
		/// Preferred blt alignment
		/// </summary>
		BLTALIGNMENT = 119
	}

	public enum COINIT : uint //tagCOINIT
	{
		COINIT_MULTITHREADED = 0x0, //Initializes the thread for multi-threaded object concurrency.
		COINIT_APARTMENTTHREADED = 0x2, //Initializes the thread for apartment-threaded object concurrency
		COINIT_DISABLE_OLE1DDE = 0x4, //Disables DDE for OLE1 support
		COINIT_SPEED_OVER_MEMORY = 0x8, //Trade memory for speed
	}

	[Flags]
	public enum CLSCTX : uint
	{
		CLSCTX_INPROC_SERVER          = 0x1,
		CLSCTX_INPROC_HANDLER         = 0x2,
		CLSCTX_LOCAL_SERVER           = 0x4,
		CLSCTX_INPROC_SERVER16        = 0x8,
		CLSCTX_REMOTE_SERVER          = 0x10,
		CLSCTX_INPROC_HANDLER16       = 0x20,
		CLSCTX_RESERVED1              = 0x40,
		CLSCTX_RESERVED2              = 0x80,
		CLSCTX_RESERVED3              = 0x100,
		CLSCTX_RESERVED4              = 0x200,
		CLSCTX_NO_CODE_DOWNLOAD       = 0x400,
		CLSCTX_RESERVED5              = 0x800,
		CLSCTX_NO_CUSTOM_MARSHAL      = 0x1000,
		CLSCTX_ENABLE_CODE_DOWNLOAD   = 0x2000,
		CLSCTX_NO_FAILURE_LOG         = 0x4000,
		CLSCTX_DISABLE_AAA            = 0x8000,
		CLSCTX_ENABLE_AAA             = 0x10000,
		CLSCTX_FROM_DEFAULT_CONTEXT   = 0x20000,
		CLSCTX_ACTIVATE_32_BIT_SERVER = 0x40000,
		CLSCTX_ACTIVATE_64_BIT_SERVER = 0x80000,
		CLSCTX_INPROC = CLSCTX_INPROC_SERVER|CLSCTX_INPROC_HANDLER,
		CLSCTX_SERVER = CLSCTX_INPROC_SERVER|CLSCTX_LOCAL_SERVER|CLSCTX_REMOTE_SERVER,
		CLSCTX_ALL    = CLSCTX_SERVER|CLSCTX_INPROC_HANDLER
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MULTI_QI
	{
		[MarshalAs(UnmanagedType.LPStruct)]  public Guid pIID;
		[MarshalAs(UnmanagedType.Interface)] public object pItf;
		public int hr;
	}
}