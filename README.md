# VideoDedup

[![Build status](https://ci.appveyor.com/api/projects/status/ld6w3vd6m49spu27/branch/master?svg=true)](https://ci.appveyor.com/project/SebastianBecker2/videodedup/branch/master)

Tool to deduplicate video files.\
A duplicate is defined as two videos that visually have the same content. For example, if the videos show the same material with different encodings, frame rates and/or resolutions.

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
- [.NET 8](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [ffmpeg](https://ffmpeg.org/)
- [SkiaSharp](https://github.com/mono/SkiaSharp)
- [Newtonsoft.Json](https://www.newtonsoft.com/json)
- [WindowsAPICodePack](https://github.com/contre/Windows-API-Code-Pack-1.1)
- The amazing [FatCow IconPack](https://www.fatcow.com/free-icons)
- [WiX Toolset 5](https://wixtoolset.org)