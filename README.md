# VideoDedup

[![Build status](https://ci.appveyor.com/api/projects/status/ld6w3vd6m49spu27/branch/master?svg=true)](https://ci.appveyor.com/project/SebastianBecker2/videodedup/branch/master)

Tool to deduplicate video files.\
A duplicate is defined as two videos that visually have the same content. For example if the videos show the same material with different encodings, frame rates and/or resolutions.

This tool includes a server and client module. By default, the client looks for the server on localhost. Communication is achieved using [gRPC](https://grpc.io/).
The server searches for duplicates of video files. Duplicates are determined by the following configurable settings:

- Number of Images to compare => Defines how many images will be compared to determine a duplicate.
- Accepted number of different Images => Defines how many of the compared images can be different to still qualify as a duplicate.
- Accepted percentage of difference => Defines how different two images have to be to be considered as different images. 80% seems to be a good value.

The comparison algorithm for images is based on [ImageComparison by Jakop Farian Krarup](https://www.codeproject.com/Articles/374386/Simple-image-comparison-in-NET). Though the library isn't used directly anymore. The algorithm is the same.

To avoid comparing videos that differ too much in length, a maximum difference in length can be defined. Either as an absolute value in seconds or in percent.

The server will search for all video files (determined by the filename extension) in a specified folder. And provides the option to monitor the folder for file changes.

The duplicates can be resolved using the client. It will view details as well as a preview of both videos. The user can decide how the duplicate shall be resolved.

VidepDedup is using the following tools and libraries:

- [gRPC](https://grpc.io/)
- [.NET 6](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-6)
- [libmpv](https://github.com/mpv-player/mpv)
- [KGySoft Drawing](https://kgysoft.net/drawing)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [WindowsAPICodePack](https://github.com/contre/Windows-API-Code-Pack-1.1)
- The amazing [FatCow IconPack](https://www.fatcow.com/free-icons)
- [WiX Toolset](https://wixtoolset.org)

libmpv 0.33 was cross-compiled with LGPL flag from the "release/0.33" branch using MXE for x64 and x86 with:  

    DEST_OS=win32 TARGET=x86_64-w64-mingw32.static ./waf configure --enable-libmpv-shared --disable-cplayer --enable-lgpl --disable-debug-build --disable-lua
and:  

    DEST_OS=win32 TARGET=i686-w64-mingw32.static ./waf configure --enable-libmpv-shared --disable-cplayer --enable-lgpl --disable-debug-build --disable-lua
respectively.  
For detailed configuration check [x64 libmpv_build_config](./DedupEngine/Libs/libmpv/x64/libmpv_build_config) and [x86 libmpv_build_config](./DedupEngine/Libs/libmpv/x86/libmpv_build_config) respectively.  
For information on how to build libmpv check out [mpv-players windows compilation guide](https://github.com/mpv-player/mpv/blob/master/DOCS/compile-windows.md).
