# Random Wallpaper #
A utility for changing the desktop background image for in Windows

## Usage ##
```
RndWallpaper [options] (file or folder) [ .. additional files and/or folders .. ]

Informational:
 -i                                     Show all information
 -im                                    Show monitor information
 -iw                                    Show current wallpapers

Wallpaper:
 -d  (interface)                        Choose which interface to use
 -w  (integer)                          Delay number of seconds (default 0)
 -s  (style)                            Style of wallpaper (default 'Fill')
 -r                                     Include subdirectories when folder is specified
 -sa [ratio]                            Detect panorama images when w/h > ratio (default 2.0)
 -m  (monitor)                          Apply image only to a single monitor
 -rs (integer)                          Random seed value (default system suplied)
 -fs                                    When folder given, use same image for all monitors

Available Styles:
 1. Center
 2. Tile
 3. Stretch
 4. Fit
 5. Fill
 6. Span

Available Interfaces:
 2. Windows (V)irtual Desktop           Change wallpaper for Windows Virtual Desktop
 1. (W)indows Legacy                    Change wallpaper using Windows legacy api
```

# Build #
* see https://github.com/dotnet/runtimelab/tree/feature/NativeAOT/samples/HelloWorld
* ```dotnet publish -r win-x64 -c release```

## TODO ##
* add better messages for hresult errors
* add more platforms (linux?)
* add an option to choose sequential images instead of just random ones from a folder
* maybe add a change settings action
* add a way to change the lock screen also
  * what about the login screen ? (is that different ?)
* maybe add generators that keep state and advance each time the background is generated
  * could be several that can be cycled through
* maybe try UWP again. ?
  * https://blog.pieeatingninjas.be/2018/04/05/creating-a-uwp-console-app-in-c/
* maybe add support for custom fitting options
  * not sure what those would be
  * seam carving resize ?

## Notes ##
* Virtual desktop interfaces constantly change :'( see these projects:
  * https://github.com/slnz00/VirtualDesktopDumper/tree/master
  * https://github.com/MScholtes/VirtualDesktop