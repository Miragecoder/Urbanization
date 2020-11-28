using System.Drawing;
using System.Threading;
using SixLabors.ImageSharp;

namespace Mirage.Urbanization.Tilesets
{
    public class VehicleBitmap : BaseBitmap
    {
        private static int _idCounter = default(int);
        public int Id { get; }
        public VehicleBitmap(Image bitmap) : base(bitmap)
        {
            Id = Interlocked.Increment(ref _idCounter);
        }
    }
}