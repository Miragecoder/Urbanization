namespace Mirage.Urbanization.ZoneConsumption.Base
{
    internal class AreaZoneConsumptionOverrideInfoResult : IGetCanOverrideWithResult
    {
        public bool WillSucceed => ResultingAreaConsumption == ToBeDeployedAreaConsumption;

        public IAreaZoneConsumption ResultingAreaConsumption { get; }
        public IAreaZoneConsumption ToBeDeployedAreaConsumption { get; }

        internal AreaZoneConsumptionOverrideInfoResult(IAreaZoneConsumption resultingAreaConsumption, IAreaZoneConsumption toBeDeployedAreaConsumption)
        {
            ResultingAreaConsumption = resultingAreaConsumption;
            ToBeDeployedAreaConsumption = toBeDeployedAreaConsumption;
        }
    }
}