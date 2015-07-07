using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    public interface IAreaConsumptionResult : IAreaMessage
    {
        bool Success { get; }
        IAreaConsumption AreaConsumption { get; }
    }
}