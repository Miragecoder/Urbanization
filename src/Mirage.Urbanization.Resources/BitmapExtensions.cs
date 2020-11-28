using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using System.Linq;
using SixLabors.Primitives;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Mirage.Urbanization.Tilesets
{
    public static class ImageExtensions
    {
        public static Image RotateImage(this Image bmp, float angle) => bmp.Clone(x => x.Rotate(angle));

        public static IEnumerable<KeyValuePair<Point, Image>> GetSegments(this Image image, int multiplier)
        {
            static Image GetImageSegment(Image image, int x, int y, int multiplier)
            {
                // Clone a portion of the Image object.
                Rectangle cloneRect = new Rectangle(
                    x: (x * multiplier),
                    y: (y * multiplier),
                    width: 1 * multiplier,
                    height: 1 * multiplier
                );

                return image.Clone(x => x.Crop(cloneRect));
            }

            return
                from x in Enumerable.Range(0, image.Width / multiplier)
                from y in Enumerable.Range(0, image.Height / multiplier)
                select new KeyValuePair<Point, Image>(new Point(x, y), GetImageSegment(image, x, y, multiplier));
        }
    }
}