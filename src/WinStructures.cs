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

		public int X {
			get { return Left; }
			set { Right -= (Left - value); Left = value; }
		}

		public int Y {
			get { return Top; }
			set { Bottom -= (Top - value); Top = value; }
		}

		public int Height {
			get { return Bottom - Top; }
			set { Bottom = value + Top; }
		}

		public int Width {
			get { return Right - Left; }
			set { Right = value + Left; }
		}

		public System.Drawing.Point Location {
			get { return new System.Drawing.Point(Left, Top); }
			set { X = value.X; Y = value.Y; }
		}

		public System.Drawing.Size Size {
			get { return new System.Drawing.Size(Width, Height); }
			set { Width = value.Width; Height = value.Height; }
		}

		public static implicit operator System.Drawing.Rectangle(RECT r) {
			return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
		}

		public static implicit operator RECT(System.Drawing.Rectangle r) {
			return new RECT(r);
		}

		public static bool operator ==(RECT r1, RECT r2) {
			return r1.Equals(r2);
		}

		public static bool operator !=(RECT r1, RECT r2) {
			return !r1.Equals(r2);
		}

		public bool Equals(RECT r) {
			return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
		}

		public override bool Equals(object obj)
		{
			if (obj is RECT rECT)
			return Equals(rECT);
			else if (obj is System.Drawing.Rectangle rectangle)
			return Equals(new RECT(rectangle));
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
		None =              0x0,
		/// <summary>The device is part of the desktop.</summary>
		AttachedToDesktop = 0x1,
		MultiDriver =       0x2,
		/// <summary>The device is part of the desktop.</summary>
		PrimaryDevice =     0x4,
		/// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
		MirroringDriver =   0x8,
		/// <summary>The device is VGA compatible.</summary>
		VGACompatible =     0x10,
		/// <summary>The device is removable; it cannot be the primary display.</summary>
		Removable =         0x20,
		/// <summary>The device has more display modes than its output devices support.</summary>
		ModesPruned =       0x8000000,
		Remote =            0x4000000,
		Disconnect =        0x2000000
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
		/// <summary>Device driver version</summary>
		DRIVERVERSION = 0,
		/// <summary>Device classification</summary>
		TECHNOLOGY = 2,
		/// <summary>Horizontal size in millimeters</summary>
		HORZSIZE = 4,
		/// <summary>Vertical size in millimeters</summary>
		VERTSIZE = 6,
		/// <summary>Horizontal width in pixels</summary>
		HORZRES = 8,
		/// <summary>Vertical height in pixels</summary>
		VERTRES = 10,
		/// <summary>Number of bits per pixel</summary>
		BITSPIXEL = 12,
		/// <summary>Number of planes</summary>
		PLANES = 14,
		/// <summary>Number of brushes the device has</summary>
		NUMBRUSHES = 16,
		/// <summary>Number of pens the device has</summary>
		NUMPENS = 18,
		/// <summary>Number of markers the device has</summary>
		NUMMARKERS = 20,
		/// <summary>Number of fonts the device has</summary>
		NUMFONTS = 22,
		/// <summary>Number of colors the device supports</summary>
		NUMCOLORS = 24,
		/// <summary>Size required for device descriptor</summary>
		PDEVICESIZE = 26,
		/// <summary>Curve capabilities</summary>
		CURVECAPS = 28,
		/// <summary>Line capabilities</summary>
		LINECAPS = 30,
		/// <summary>Polygonal capabilities</summary>
		POLYGONALCAPS = 32,
		/// <summary>Text capabilities</summary>
		TEXTCAPS = 34,
		/// <summary>Clipping capabilities</summary>
		CLIPCAPS = 36,
		/// <summary>Bitblt capabilities</summary>
		RASTERCAPS = 38,
		/// <summary>Length of the X leg</summary>
		ASPECTX = 40,
		/// <summary>Length of the Y leg</summary>
		ASPECTY = 42,
		/// <summary>Length of the hypotenuse</summary>
		ASPECTXY = 44,
		/// <summary>Shading and Blending caps</summary>
		SHADEBLENDCAPS = 45,
		/// <summary>Logical pixels inch in X</summary>
		LOGPIXELSX = 88,
		/// <summary>Logical pixels inch in Y</summary>
		LOGPIXELSY = 90,
		/// <summary>Number of entries in physical palette</summary>
		SIZEPALETTE = 104,
		/// <summary>Number of reserved entries in palette</summary>
		NUMRESERVED = 106,
		/// <summary>Actual color resolution</summary>
		COLORRES = 108,
		// Printing related DeviceCaps. These replace the appropriate Escapes
		/// <summary>Physical Width in device units</summary>
		PHYSICALWIDTH = 110,
		/// <summary>Physical Height in device units</summary>
		PHYSICALHEIGHT = 111,
		/// <summary>Physical Printable Area x margin</summary>
		PHYSICALOFFSETX = 112,
		/// <summary>Physical Printable Area y margin</summary>
		PHYSICALOFFSETY = 113,
		/// <summary>Scaling factor x</summary>
		SCALINGFACTORX = 114,
		/// <summary>Scaling factor y</summary>
		SCALINGFACTORY = 115,
		/// <summary>Current vertical refresh rate of the display device (for displays only) in Hz</summary>
		VREFRESH = 116,
		/// <summary>Vertical height of entire desktop in pixels</summary>
		DESKTOPVERTRES = 117,
		/// <summary>Horizontal width of entire desktop in pixels</summary>
		DESKTOPHORZRES = 118,
		/// <summary>Preferred blt alignment</summary>
		BLTALIGNMENT = 119
	}

	public enum COINIT : uint //tagCOINIT
	{
		MULTITHREADED =     0x0, //Initializes the thread for multi-threaded object concurrency.
		APARTMENTTHREADED = 0x2, //Initializes the thread for apartment-threaded object concurrency
		DISABLE_OLE1DDE =   0x4, //Disables DDE for OLE1 support
		SPEED_OVER_MEMORY = 0x8, //Trade memory for speed
	}

	[Flags]
	public enum CLSCTX : uint
	{
		INPROC_SERVER          = 0x00001,
		INPROC_HANDLER         = 0x00002,
		LOCAL_SERVER           = 0x00004,
		INPROC_SERVER16        = 0x00008,
		REMOTE_SERVER          = 0x00010,
		INPROC_HANDLER16       = 0x00020,
		RESERVED1              = 0x00040,
		RESERVED2              = 0x00080,
		RESERVED3              = 0x00100,
		RESERVED4              = 0x00200,
		NO_CODE_DOWNLOAD       = 0x00400,
		RESERVED5              = 0x00800,
		NO_CUSTOM_MARSHAL      = 0x01000,
		ENABLE_CODE_DOWNLOAD   = 0x02000,
		NO_FAILURE_LOG         = 0x04000,
		DISABLE_AAA            = 0x08000,
		ENABLE_AAA             = 0x10000,
		FROM_DEFAULT_CONTEXT   = 0x20000,
		ACTIVATE_32_BIT_SERVER = 0x40000,
		ACTIVATE_64_BIT_SERVER = 0x80000,
		INPROC = INPROC_SERVER | INPROC_HANDLER,
		SERVER = INPROC_SERVER | LOCAL_SERVER | REMOTE_SERVER,
		ALL    = SERVER | INPROC_HANDLER
	}

	[Flags]
	public enum FormatMessageFlags : uint
	{
		ALLOCATE_BUFFER = 0x00000100,
		IGNORE_INSERTS =  0x00000200,
		FROM_STRING =     0x00000400,
		FROM_HMODULE =    0x00000800,
		FROM_SYSTEM =     0x00001000,
		ARGUMENT_ARRAY =  0x00002000
	}

	public enum DisplayTechnologyType
	{
		ShadowMaskCathodeRayTube    = 0,
		ApertureGrillCathodeRayTube = 1,
		ThinFilmTransistor          = 2,
		LiquidCrystalOnSilicon      = 3,
		Plasma                      = 4,
		OrganicLightEmittingDiode   = 5,
		Electroluminescent          = 6,
		Microelectromechanical      = 7,
		FieldEmissionDevice         = 8
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct PHYSICAL_MONITOR
	{
		public IntPtr hPhysicalMonitor;
		const int PhysicalMonitorDescriptionSize = 128;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = PhysicalMonitorDescriptionSize)]
		public string szPhysicalMonitorDescription;
	}

	[Flags]
	public enum MC_CAPS : uint
	{
		None                                         = 0x0000,
		MonitorTechnologyType                        = 0x0001,
		Brightness                                   = 0x0002,
		Contrast                                     = 0x0004,
		ColorTemperature                             = 0x0008,
		RedGreenBlueGain                             = 0x0010,
		RedGreenBlueDrive                            = 0x0020,
		Degauss                                      = 0x0040,
		DisplayAreaPosition                          = 0x0080,
		DisplayAreaSize                              = 0x0100,
		//missing 0x200 (maybe reserved ?)
		RestoreFactoryDefaults                       = 0x0400,
		RestoreFactoryColorDefaults                  = 0x0800,
		RestoreFactoryDefaultsEnablesMonitorSettings = 0x1000
	}

	public enum ColorTemperature : uint
	{
		TemperatureUnknown = 0,
		Temperature4000K   = 1,
		Temperature5000K   = 2,
		Temperature6500K   = 3,
		Temperature7500K   = 4,
		Temperature8200K   = 5,
		Temperature9300K   = 6,
		Temperature10000K  = 7,
		Temperature11500K  = 8
	};

	[Flags]
	public enum ColorTemperatureSupported : uint
	{
		TemperatureNone   = 0x00,
		Temperature4000K  = 0x01,
		Temperature5000K  = 0x02,
		Temperature6500K  = 0x04,
		Temperature7500K  = 0x08,
		Temperature8200K  = 0x10,
		Temperature9300K  = 0x20,
		Temperature10000K = 0x40,
		Temperature11500K = 0x80
	};
}