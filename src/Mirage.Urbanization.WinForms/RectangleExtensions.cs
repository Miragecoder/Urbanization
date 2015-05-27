using System;
using System.Drawing;

namespace Mirage.Urbanization.WinForms
{
    public static class RectangleExtensions
    {
        public static Rectangle InflateAndReturn(this Rectangle rectangle, int width, int height)
        {
            rectangle.Inflate(width, height);
            return rectangle;
        }
        public static Rectangle ChangeSize(this Rectangle rectangle, Size size)
        {
            rectangle.Size = size;
            return rectangle;
        }

        public static Rectangle Relocate(this Rectangle rectangle, Func<Point, Point> relocateAction)
        {
            rectangle.Location = relocateAction(rectangle.Location);
            return rectangle;
        }
    }
}