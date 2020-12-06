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
		Center = 1,
		Tile = 2,
		Stretch = 3,
		Fit = 4,
		Fill = 5,
		Span = 6
	}

	//special enum used to select monitor
	public enum PickMonitor
	{
		All = 0,
		Primary = -1,
	}

}