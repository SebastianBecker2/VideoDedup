# VideoDedup
Tool to deduplicate video files

Branch|[![AppVeyor logo](pics/AppVeyor.png)](https://appveyor.com)
---|---
master|[![Build status](https://ci.appveyor.com/api/projects/status/ld6w3vd6m49spu27/branch/master?svg=true)](https://ci.appveyor.com/project/SebastianBecker2/videodedup/branch/master)

Using the following libraries:
- libmpv (https://github.com/mpv-player/mpv)
- Newtonsoft.Json
- WindowsAPICodePack
- ImageComparison by Jakob Farian Krarup (https://www.codeproject.com/Articles/374386/Simple-image-comparison-in-NET)
- FatCow IconPack (http://www.fatcow.com/free-icons)

libmpv 0.33 was cross-compiled with LGPL flag from the "release/0.33" branch using MXE for x64 and x86 with:  
DEST_OS=win32 TARGET=x86_64-w64-mingw32.static ./waf configure --enable-libmpv-shared --disable-cplayer --enable-lgpl --disable-debug-build --disable-lua  
and:  
DEST_OS=win32 TARGET=i686-w64-mingw32.static ./waf configure --enable-libmpv-shared --disable-cplayer --enable-lgpl --disable-debug-build --disable-lua  
respectively.  
For detailed configuration check [x64 libmpv_build_config](./VideoDedupShared/Libs/libmpv/x64/libmpv_build_config) and [x86 libmpv_build_config](./VideoDedupShared/Libs/libmpv/x86/libmpv_build_config) respectively.  
For information on how to build libmpv check out [mpv-players windows compilation guide](https://github.com/mpv-player/mpv/blob/master/DOCS/compile-windows.md).
