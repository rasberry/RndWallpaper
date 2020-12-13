namespace RndWallpaper
{
	public enum PickReason
	{
		Success = 0,
		FileDoesNotExist,
		NotSupported,
		SetWallpaperFailed,
		FileNotAvailable
	}

	public enum PickWallpaperStyle
	{
		None = 0,
		Center,
		Tile,
		Stretch,
		Fit,
		Fill,
		Span
	}

	//special enum used to select monitor
	public enum PickMonitor
	{
		All = 0,
		Primary = -1,
	}

}