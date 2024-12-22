using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using FfmpegLib;


static void ClearFolder(string path)
{
    try
    {
        var directoryInfo = new DirectoryInfo(path);

        foreach (var file in directoryInfo.GetFiles())
        {
            file.Delete();
        }

        foreach (var dir in directoryInfo.GetDirectories())
        {
            dir.Delete(true);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while clearing the folder: {ex.Message}");
    }
}
static void SaveByteArrayAsImage(byte[] imageBytes, string filePath)
{
    using (var ms = new MemoryStream(imageBytes))
    {
        using (var image = Image.FromStream(ms))
        {
            image.Save(filePath, ImageFormat.Png); // You can choose ImageFormat.Jpeg or ImageFormat.Bmp
        }
    }
}

static IEnumerable<string> GetAllAccessibleFilesIn(
    string rootDirectory,
    IEnumerable<string>? excludedDirectories = null,
    bool recursive = true,
    string searchPattern = "*.*")
{
    if (Path.GetFileName(rootDirectory) == "$RECYCLE.BIN")
    {
        return [];
    }

    IEnumerable<string> files = [];
    excludedDirectories ??= [];

    try
    {
        files = files.Concat(Directory.EnumerateFiles(rootDirectory,
            searchPattern, SearchOption.TopDirectoryOnly));

        if (recursive)
        {
            foreach (var directory in Directory
                .GetDirectories(rootDirectory)
                .Where(d => !excludedDirectories.Contains(d,
                    StringComparer.InvariantCultureIgnoreCase)))
            {
                files = files.Concat(GetAllAccessibleFilesIn(directory,
                    excludedDirectories, recursive, searchPattern));
            }
        }
    }
    catch (UnauthorizedAccessException)
    {
        // Don't do anything if we cannot access a file.
    }

    return files;
}

var fileEndings = new List<string> { ".mp4" };

var basePath = "\\\\bowser\\data";
var files = GetAllAccessibleFilesIn(
    basePath,
    null,
    false)
    .Where(f => fileEndings.Contains(
        Path.GetExtension(f),
        StringComparer.InvariantCultureIgnoreCase))
    .Take(100)
    .Order()
    //.SkipWhile(f => f != "\\\\bowser\\data\\MFC\\soft doll small\\soft_doll_small chaturbate webcam video recording Sept 30 2024.mp4")
    .Where(f => f != "\\\\bowser\\data\\MFC\\soft doll small\\soft_doll_small chaturbate webcam video recording Sept 30 2024.mp4")
    .Where(f => f != "\\\\bowser\\data\\MFC\\soft doll small\\soft_doll_small chaturbate webcam video recording Sept 19 2024.mp4")
    .Where(f => f != "\\\\bowser\\data\\MFC\\AvrilDoll\\AvrilDollX some nudity.mp4")
    .Where(f => f != "\\\\bowser\\data\\MFC\\KimTylor\\Recorded\\Nicole\\kim_Nicole Kimtylor NicoleHitman - 10_03_13.mp4")
    .Where(f => f != "\\\\bowser\\data\\Fetish\\Instructions\\Lissie Belle\\lissie belle the bitch cure.mp4 at Streamtape.com.mp4")
    .Where(f => f != "\\\\bowser\\data\\MFC\\Audrey & Foxy\\Recorded\\Chaturbate - Audrey.mp4")
    .Where(f => f != "\\\\bowser\\data\\MFC\\Dawnwillow (Dawn Willow)\\Dawnwillow 2015-01-24.mp4")
    .ToList();


//files = ["\\\\bowser\\data\\Fetish\\Instructions\\Bratty Bunny\\Watch Bratty Bunny Cuckold The Door mp4.mp4"];
//files = ["\\\\bowser\\data\\MFC\\AvrilDoll\\AvrilDollX some nudity.mp4"];
//files = ["\\\\bowser\\data\\Avril 220621 DildoFuck&Riding.mp4"];
//files = ["D:\\VideoDedupTest\\SampleVideo_720x480_5mb.mp4"];
//files = ["\\\\bowser\\data\\C6CEF8D.mp4.mp4"];
//files = ["\\\\bowser\\data\\MFC\\Sassyt33n\\Watch sassyt33ns Chaturbate show from 1 week ago.mp4"];
files = ["\\\\bowser\\data\\MFC\\Hanna Costello\\Performer hanna_costtello show on 2023-08-14 0510, Chaturbate Archive – Recurbate.mp4"];

var imageCount = 10;


//{
//    foreach (var file in files)
//    {
//        var ffmpeg = new FfmpegWrapper(file);
//        var ffmpegImages = ffmpeg.GetImages(0, imageCount, imageCount);
//        var ffmpegCounter = 0;
//        ClearFolder($"D:\\VideoDedupTest\\temp\\ffmpeg\\");
//        foreach (var image in ffmpegImages)
//        {
//            if (image is null)
//            {
//                continue;
//            }

//            SaveByteArrayAsImage(image, $"D:\\VideoDedupTest\\temp\\ffmpeg\\file{ffmpegCounter++}.png");
//        }

//        var mpv = new MpvLib.MpvWrapper(file);
//        var mpvImages = mpv.GetImages(0, imageCount, imageCount);
//        var mpvCounter = 0;
//        ClearFolder($"D:\\VideoDedupTest\\temp\\mpv\\");
//        foreach (var image in mpvImages)
//        {
//            if (image is null)
//            {
//                continue;
//            }

//            SaveByteArrayAsImage(image, $"D:\\VideoDedupTest\\temp\\mpv\\file{mpvCounter++}.png");
//        }

//        Console.ReadLine();
//    }
//}

{
    Console.WriteLine($"FFMPEG");
    var stopwatch = Stopwatch.StartNew();
    foreach (var file in files)
    {
        var ffmpeg = new FfmpegWrapper(file);
        var ffmpegImages = ffmpeg.GetImages(3, 5, imageCount);
        Console.WriteLine($"FFMPEG: {ffmpegImages.Count()}");
        var ffmpegCounter = 0;
        ClearFolder($"D:\\VideoDedupTest\\temp\\ffmpeg\\");
        foreach (var image in ffmpegImages)
        {
            if (image is null)
            {
                continue;
            }
            SaveByteArrayAsImage(image, $"D:\\VideoDedupTest\\temp\\ffmpeg\\file{ffmpegCounter++}.png");
        }
    }
    Console.WriteLine($"FFMPEG took: {stopwatch.ElapsedMilliseconds} ms");
}

{
    Console.WriteLine($"libMPV");
    var stopwatch = Stopwatch.StartNew();
    foreach (var file in files)
    {
        using var mpv = new MpvLib.MpvWrapper(file);
        var mpvImages = mpv.GetImages(3, 5, imageCount);
        var mpvCounter = 0;
        ClearFolder($"D:\\VideoDedupTest\\temp\\mpv\\");
        foreach (var image in mpvImages)
        {
            if (image is null)
            {
                continue;
            }
            SaveByteArrayAsImage(image, $"D:\\VideoDedupTest\\temp\\mpv\\file{mpvCounter++}.png");
        }
    }
    Console.WriteLine($"libMPV took: {stopwatch.ElapsedMilliseconds} ms");
}

//Console.WriteLine($"MPV");
//var mpv = new MpvLib.MpvWrapper(files[0]);
//var mpvImages = mpv.GetImages(0, imageCount, imageCount);
//var mpvCounter = 0;
//ClearFolder($"D:\\VideoDedupTest\\temp\\mpv\\");
//foreach (var image in mpvImages)
//{
//    if (image is null)
//    {
//        continue;
//    }
//    SaveByteArrayAsImage(image, $"D:\\VideoDedupTest\\temp\\mpv\\file{mpvCounter++}.png");
//}

//Console.WriteLine($"FFMPEG");
//var ffmpeg = new FfmpegWrapper(files[0]);
//var ffmpegImages = ffmpeg.GetImages(0, imageCount, imageCount);
//var ffmpegCounter = 0;
//ClearFolder($"D:\\VideoDedupTest\\temp\\ffmpeg\\");
//foreach (var image in ffmpegImages)
//{
//    if (image is null)
//    {
//        continue;
//    }
//    SaveByteArrayAsImage(image, $"D:\\VideoDedupTest\\temp\\ffmpeg\\file{ffmpegCounter++}.png");
//}






//Console.WriteLine($"MpvLib: {string.Join(" ", mpvImages.First().Take(10).Select(b => b.ToString("X2")))}");
//Console.WriteLine($"Ffmpeg: {string.Join(" ", ffmpegImages.First().Take(10).Select(b => b.ToString("X2")))}");

return;

{
    var stopwatch = Stopwatch.StartNew();
    var mpvLibCodecInfo = files.Select(MpvLib.MpvWrapper.GetCodecInfo).ToList();
    stopwatch.Stop();
    Console.WriteLine($"MpvLib took: {stopwatch.ElapsedMilliseconds} ms");

    stopwatch.Restart();
    var ffmpegCodecInfo = files.Select(FfmpegWrapper.GetCodecInfo).ToList();
    stopwatch.Stop();
    Console.WriteLine($"FFmpeg took: {stopwatch.ElapsedMilliseconds} ms");

    foreach (var index in Enumerable.Range(0, ffmpegCodecInfo.Count))
    {
        if (ffmpegCodecInfo[index].Size.Width == mpvLibCodecInfo[index].Size.Width
            && ffmpegCodecInfo[index].Size.Height == mpvLibCodecInfo[index].Size.Height
            && mpvLibCodecInfo[index].Name.StartsWith(ffmpegCodecInfo[index].Name, StringComparison.CurrentCultureIgnoreCase)
            && ffmpegCodecInfo[index].FrameRate - mpvLibCodecInfo[index].FrameRate <= 1)
        {
            continue;
        }
        Console.WriteLine($"{files[index]} FFmpeg: {ffmpegCodecInfo[index]} MpvLib: {mpvLibCodecInfo[index]}");
    }
}

return;

{
    var stopwatch = Stopwatch.StartNew();
    var ffmpegDurations = files.Select(FfmpegWrapper.GetDuration).ToList();
    stopwatch.Stop();
    Console.WriteLine($"FFmpeg took: {stopwatch.ElapsedMilliseconds} ms");

    stopwatch.Restart();
    var mpvLibDurations = files.Select(MpvLib.MpvWrapper.GetDuration).ToList();
    stopwatch.Stop();
    Console.WriteLine($"MpvLib took: {stopwatch.ElapsedMilliseconds} ms");

    ffmpegDurations = ffmpegDurations.Select(d => new TimeSpan(d.Hours, d.Minutes, d.Seconds)).ToList();

    foreach (var index in Enumerable.Range(0, ffmpegDurations.Count))
    {
        if (ffmpegDurations[index] != mpvLibDurations[index])
        {
            Console.WriteLine($"{files[index]} FFmpeg: {ffmpegDurations[index]} MpvLib: {mpvLibDurations[index]}");
        }
    }
    Console.WriteLine($"Results SequenceEqual: {ffmpegDurations.SequenceEqual(mpvLibDurations)}");
    Console.WriteLine("asdf");
}
