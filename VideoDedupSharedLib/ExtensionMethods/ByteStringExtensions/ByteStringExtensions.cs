namespace VideoDedupSharedLib.ExtensionMethods.ByteStringExtensions
{
    using System.Drawing;
    using System.IO;
    using Google.Protobuf;

    public static class ByteStringExtensions
    {
        public static Image ToImage(this ByteString byteString)
        {
            var stream = new MemoryStream(byteString.ToByteArray());
            return Image.FromStream(stream);
        }
    }
}
