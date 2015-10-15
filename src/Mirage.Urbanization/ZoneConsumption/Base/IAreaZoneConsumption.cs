using System.Drawing;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public interface IAreaZoneConsumption : IAreaConsumption
    {
        Color Color { get; }
        string ColorName { get; }
        IGetCanOverrideWithResult GetCanOverrideWith(IAreaZoneConsumption consumption);
    }
}