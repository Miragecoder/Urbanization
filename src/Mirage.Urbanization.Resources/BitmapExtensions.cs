using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Mirage.Urbanization.Tilesets
{
    public static class BitmapExtensions
    {
        public static Bitmap Get90DegreesRotatedClone(this Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));
            var clone = bitmap.Clone() as Bitmap;
            if (clone == null)
                throw new ArgumentException(
                    String.Format("Could not clone 'bitmap' into a new {0} instance.", typeof(Bitmap).Name), nameof(bitmap));

            clone.RotateFlip(RotateFlipType.Rotate90FlipNone);
            return clone;
        }

        public static Bitmap RotateImage(this Bitmap bmp, float angle)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2); //set the rotation point as the center into the matrix
                g.RotateTransform(angle); //rotate
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2); //restore rotation point into the matrix
                g.DrawImage(bmp, new Point(0, 0)); //draw the image on the new bitmap
            }

            return rotatedImage;
        }

        public static Bitmap RotateTrainImage(this Bitmap bmp, float angle)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width * 2, bmp.Height * 2);
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2); //set the rotation point as the center into the matrix
                g.RotateTransform(angle); //rotate
                g.TranslateTransform(-(bmp.Width / 2), -(bmp.Height / 2)); //restore rotation point into the matrix
                g.DrawImage(bmp, new Point(0, 0)); //draw the image on the new bitmap
            }

            return rotatedImage;
        }

        public static Bitmap GetBitmapSegment(Bitmap image, int x, int y, int multiplier)
        {
            // Clone a portion of the Bitmap object.
            Rectangle cloneRect = new Rectangle(
                x: (x * multiplier), 
                y: (y * multiplier), 
                width: 1 * multiplier, 
                height: 1 * multiplier
            );
            System.Drawing.Imaging.PixelFormat format = image.PixelFormat;
            return image.Clone(cloneRect, format);
        }

        public static IEnumerable<KeyValuePair<Point, Bitmap>> GetSegments(this Bitmap image, int multiplier)
        {
            return 
                from x in Enumerable.Range(0, image.Width / multiplier)
                from y in Enumerable.Range(0, image.Height / multiplier)
                select new KeyValuePair<Point, Bitmap>(new Point(x,y),  GetBitmapSegment(image, x, y, multiplier));
        }
    }
}