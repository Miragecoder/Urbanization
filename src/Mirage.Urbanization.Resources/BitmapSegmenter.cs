using System.Collections.Generic;
using System.Drawing;

namespace Mirage.Urbanization.Tilesets
{
    public class BitmapSegmenter
    {
        private readonly IDictionary<Bitmap, IDictionary<int, Bitmap>> _cache = new Dictionary<Bitmap, IDictionary<int, Bitmap>>();

        public Bitmap GetSegment(Bitmap image, int x, int y, int multiplier)
        {
            if (!_cache.ContainsKey(image))
            {
                _cache.Add(image, new Dictionary<int, Bitmap>());
            }
            int hash = ("x =" + x + "y = "+ y).GetHashCode();
            if (!_cache[image].ContainsKey(hash))
            {
                _cache[image].Add(hash, GetBitmapSegment(image, x, y, multiplier));
            }
            return _cache[image][hash];
        }

        private static Bitmap GetBitmapSegment(Bitmap image, int x, int y, int multiplier)
        {
            // Clone a portion of the Bitmap object.
            Rectangle cloneRect = new Rectangle((x * multiplier) - multiplier, (y * multiplier) - multiplier, 1 * multiplier, 1 * multiplier);
            System.Drawing.Imaging.PixelFormat format = image.PixelFormat;
            return image.Clone(cloneRect, format);
        }
    }
}