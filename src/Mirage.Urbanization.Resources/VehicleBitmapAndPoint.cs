using Mirage.Urbanization.Vehicles;

namespace Mirage.Urbanization.Tilesets
{
    public class VehicleBitmapAndPoint
    {
        public VehicleBitmapAndPoint(
            VehicleBitmap bitmap,
            IReadOnlyZoneInfo second,
            IReadOnlyZoneInfo third,
            IVehicle vehicle)
        {
            Bitmap = bitmap;
            Second = second;
            Third = third;
            Vehicle = vehicle;
        }

        public IReadOnlyZoneInfo Second { get; }
        public IReadOnlyZoneInfo Third { get; }
        public VehicleBitmap Bitmap { get; }
        public IVehicle Vehicle { get; }
    }
}