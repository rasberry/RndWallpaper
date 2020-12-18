# Random Wallpaper #
A utility for changing the desktop background image for in Windows

## Usage ##
```
RndWallpaper [options] (path of image or folder)
Options:
 -d  (number)                 Delay number of seconds (default 0)
 -s  (style)                  Style of wallpaper (default 'Fill')
 -m  (monitor)                Apply image only to a single monitor
 -rs (integer)                Random seed value (default system suplied)
 -fs                          When folder given, use same image for all monitors

Available Styles:
 1. Center
 2. Tile
 3. Stretch
 4. Fit
 5. Fill
 6. Span

Available Monitors:
 Primary                      Select the primary monitor
 1. \\.\DISPLAY1 (Primary)    Depth:32 Size:{X=0,Y=0,Width=1024,Height=768}
 2. \\.\DISPLAY2              Depth:32 Size:{X=1024,Y=0,Width=1024,Height=768}
 ...
```

# Build #
* see https://github.com/dotnet/runtimelab/tree/feature/NativeAOT/samples/HelloWorld
* ```dotnet publish -r win-x64 -c release```

## TODO ##
* add better messages for hresult errors
* add more platforms (linux?)
* add an option to choose sequential images instead of just random ones from a folder
* maybe add a way to setup an image rss feed by changing windows settings
* maybe add an information action which shows current settings
* maybe add a change settings action
* maybe add a way to download images from some sources (not sure where tho)
* add a way to change the lock screen also
  * what about the login screen ? (is that different ?)
