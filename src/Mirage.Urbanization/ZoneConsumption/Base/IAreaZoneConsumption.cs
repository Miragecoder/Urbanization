using SixLabors.ImageSharp;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public interface IAreaZoneConsumption : IAreaConsumption
    {
        Color Color { get; }
        IGetCanOverrideWithResult GetCanOverrideWith(IAreaZoneConsumption consumption);
    }
}