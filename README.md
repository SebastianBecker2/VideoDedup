# VideoDedup
Tool to deduplicate video files

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
For detailed configuration check VideoDedupShared/Libs/libmpv/x64/libmpv_build_config and VideoDedupShared/Libs/libmpv/x86/libmpv_build_config respectively.
For information on how to build libmpv check out https://github.com/mpv-player/mpv/blob/master/DOCS/compile-windows.md