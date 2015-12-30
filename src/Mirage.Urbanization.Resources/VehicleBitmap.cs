using System.Drawing;
using System.Threading;

namespace Mirage.Urbanization.Tilesets
{
    public class VehicleBitmap : BaseBitmap
    {
        private static int _idCounter = default(int);
        public int Id { get; }
        public VehicleBitmap(Bitmap bitmap) : base(bitmap)
        {
            Id = Interlocked.Increment(ref _idCounter);
        }
    }
}