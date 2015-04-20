namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public interface IGetCanOverrideWithResult
    {
        bool WillSucceed { get; }
        IAreaZoneConsumption ToBeDeployedAreaConsumption { get; }
        IAreaZoneConsumption ResultingAreaConsumption { get; }
    }
}