using System;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public class ConsumeAreaOperation : IConsumeAreaOperation
    {
        private readonly IGetCanOverrideWithResult _canOverrideWithResult;
        private readonly Action<IAreaZoneConsumption> _applyAction;

        public ConsumeAreaOperation(IGetCanOverrideWithResult canOverrideWithResult, Action<IAreaZoneConsumption> applyAction)
        {
            _canOverrideWithResult = canOverrideWithResult;
            _applyAction = applyAction;
        }

        public IGetCanOverrideWithResult CanOverrideWithResult { get { return _canOverrideWithResult; } }

        public void Apply()
        {
            if (CanOverrideWithResult.WillSucceed)
                _applyAction(_canOverrideWithResult.ToBeDeployedAreaConsumption);
            else 
                throw new InvalidOperationException();
        }

        public string Description
        {
            get { return CanOverrideWithResult.WillSucceed
                ? String.Format("{0} deployed succesfully.", _canOverrideWithResult.ToBeDeployedAreaConsumption.Name)
                : String.Format("{0} deployment failed; cannot build on {1}.", 
                CanOverrideWithResult.ToBeDeployedAreaConsumption.Name, 
                CanOverrideWithResult.ResultingAreaConsumption.Name); }
        }
    }
}