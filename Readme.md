# Random Wallpaper #
A utility for changing the desktop background image for in Windows

## Usage ##
```
RndWallpaper [options] (path of image or folder)
Options:
 -s (style)         Style of wallpaper (default 'Fill')
 -rs (integer)      Random seed value (default system suplied)

Available Styles:
 Center
 Stretch
 Fit
 Fill
 Span
```

# Build #
* see https://github.com/dotnet/runtimelab/tree/feature/NativeAOT/samples/HelloWorld
* ```dotnet publish -r win-x64 -c release```

## TODO ##
* add more platforms (linux?)
* add an option to choose sequential images instead of just random ones from a folder
* maybe add a way to setup an image rss feed by changing windows settings
* add support for choosing a monitor for multi-monitor setups
* maybe add an information action which shows current settings
* maybe add a change settings action
* maybe add a way to download images from some sources (not sure where tho)
