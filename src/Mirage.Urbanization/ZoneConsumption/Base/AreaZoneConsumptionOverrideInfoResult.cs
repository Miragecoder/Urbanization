namespace Mirage.Urbanization.ZoneConsumption.Base
{
    internal class AreaZoneConsumptionOverrideInfoResult : IGetCanOverrideWithResult
    {
        private readonly IAreaZoneConsumption _resultingAreaConsumption, _toBeDeployedAreaConsumption;

        public bool WillSucceed { get { return _resultingAreaConsumption == _toBeDeployedAreaConsumption; } }

        public IAreaZoneConsumption ResultingAreaConsumption { get { return _resultingAreaConsumption; } }
        public IAreaZoneConsumption ToBeDeployedAreaConsumption { get { return _toBeDeployedAreaConsumption; } }

        internal AreaZoneConsumptionOverrideInfoResult(IAreaZoneConsumption resultingAreaConsumption, IAreaZoneConsumption toBeDeployedAreaConsumption)
        {
            _resultingAreaConsumption = resultingAreaConsumption;
            _toBeDeployedAreaConsumption = toBeDeployedAreaConsumption;
        }
    }
}