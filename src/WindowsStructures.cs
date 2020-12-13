using System;
using System.Runtime.InteropServices;

namespace RndWallpaper
{
	public enum UAction
	{
		SPI_SETDESKWALLPAPER = 20,
		SPI_GETDESKWALLPAPER = 115
	}

	[Flags]
	public enum SPIF
	{
		UPDATEINIFILE = 0x01,
		SENDWININICHANGE = 0x02
	}

/*
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	internal sealed class MONITORINFO
	{
		public Int32 m_cbSize;
		public RECT m_rcMonitor;
		public RECT m_rcWork;
		public Int32 m_dwFlags;

		public MONITORINFO()
		{
			m_cbSize = Marshal.SizeOf(typeof(MONITORINFO));
			m_rcMonitor = new RECT();
			m_rcWork = new RECT();
			m_dwFlags = 0;
		}
	}
*/

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

	[StructLayout(LayoutKind.Sequential)]
	public struct ImmersiveColorPreference
	{
		public uint Color1; //COLORREF - StartColorMenu
		public uint Color2; //COLORREF - AccentColorMenu
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

	[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
	public struct DISPLAY_DEVICE
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
}