using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace Mirage.Urbanization.WinForms
{
    public static class ImageSharpExtensions
    {
        public static System.Drawing.Color ToSysDrawingColor(this SixLabors.ImageSharp.Color color)
            => System.Drawing.ColorTranslator.FromHtml("#" + color.ToHex().Substring(0,6));

        private static ConcurrentDictionary<SixLabors.ImageSharp.Image, System.Drawing.Bitmap> _bitmapCache 
            = new ConcurrentDictionary<SixLabors.ImageSharp.Image, Bitmap>();

        public static System.Drawing.Bitmap ToSysDrawingBitmap(this SixLabors.ImageSharp.Image image) 
        {
            return _bitmapCache.GetOrAdd(image, i => ToBitmapPrivate(image));
        }

        private static System.Drawing.Bitmap ToBitmapPrivate(this SixLabors.ImageSharp.Image image) 
        {
            using var memoryStream = new MemoryStream();
            var imageEncoder = image.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);
            image.Save(memoryStream, imageEncoder);

            memoryStream.Seek(0, SeekOrigin.Begin);

            return new System.Drawing.Bitmap(memoryStream);
        }
    }
}