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
		Tile,
		Center,
		Stretch,
		Fit,
		Fill,
		Span
	}
}