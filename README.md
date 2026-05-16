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
- [WiX Toolset 6](https://wixtoolset.org)

## Two-PC setup (server and client on different machines)

The client talks to the server over **HTTPS** with a **self-signed** server certificate. The client does **not** use the Windows Trusted Root store for that server; it **pins** the exact public certificate (`VideoDedup.crt`).

1. **Server PC** — Run the **bundle** installer (`SetupBootstrap` output), enable **Server**, complete setup. On the **Server connectivity** step you can keep the default listen port (**51726**) or choose another TCP port (1024–65535), and choose **All networks** (default, `[::]`) or one or more specific adapter addresses. Remote clients must use the same port and an address the server is actually listening on. When installation finishes, the bootstrapper shows where `VideoDedup.crt` is stored. Use **Save copy as…** or **Open folder** to copy it to a USB drive or network share. If you install **Client** and **Server** on the same PC, the client’s default port is preset to match the server.
2. **Client PC** — Run the same bundle, enable **Client** only (or install Client with Server unchecked). Browse to the `VideoDedup.crt` you copied, or skip and import later.
3. **Later / certificate rotation** — Re-run the bundle on the client PC and choose **Import server certificate…** (maintenance), or in the running client use **Client Configuration** to set the certificate path, or accept the prompt if the connection fails after a server reinstall.

The server’s public certificate is also written under `%ProgramFiles%\VideoDedupServer\cert\` (alongside the private key used by the service). The installed client expects `cert\VideoDedup.crt` next to `VideoDedupClient.exe` unless you override **Server certificate** in Client Configuration (saved under your user profile).

### Linux server (DEB/RPM/Arch)

Linux packages generate the same self-signed material at install time (or on first start for Flatpak/Snap). The gRPC endpoint is **HTTPS** on port **51726**.

- Private key: `/usr/lib/videodedupserver/cert/VideoDedup.pfx` (password in `/etc/videodedupserver/tls.env`, read by systemd)
- Public cert for clients: `/usr/lib/videodedupserver/cert/VideoDedup.crt`

Copy `VideoDedup.crt` to the client machine and import it (Client Configuration → **Server certificate**, or place under `cert\VideoDedup.crt` next to the client binary). Use protocol **https** and port **51726**.