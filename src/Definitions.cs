namespace RndWallpaper
{
	public enum PickWallpaperStyle
	{
		None = 0,
		Center,
		Tile,
		Stretch,
		Fit,
		Fill,
		Span = 6
	}

	//special enum used to select monitor
	public enum PickMonitor
	{
		All = 0,
		Primary = -1,
	}

	public enum PickAction
	{
		None = 0,
		Wallpaper = 1,
		Info = 2,
		Download = 3
	}

	public enum PickSource
	{
		None = 0,
		Bing = 1,
		RSS = 2,
		Windows = 3
	}
}