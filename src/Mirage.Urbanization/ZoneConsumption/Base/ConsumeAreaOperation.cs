using System;

namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public class ConsumeAreaOperation : IConsumeAreaOperation
    {
        private readonly Action<IAreaZoneConsumption> _applyAction;

        public ConsumeAreaOperation(IGetCanOverrideWithResult canOverrideWithResult, Action<IAreaZoneConsumption> applyAction)
        {
            CanOverrideWithResult = canOverrideWithResult;
            _applyAction = applyAction;
        }

        public IGetCanOverrideWithResult CanOverrideWithResult { get; }

        public void Apply()
        {
            if (CanOverrideWithResult.WillSucceed)
                _applyAction(CanOverrideWithResult.ToBeDeployedAreaConsumption);
            else 
                throw new InvalidOperationException();
        }

        public string Description => CanOverrideWithResult.WillSucceed
            ? String.Format("{0} deployed succesfully.", CanOverrideWithResult.ToBeDeployedAreaConsumption.Name)
            : String.Format("{0} deployment failed; cannot build on {1}.", 
                CanOverrideWithResult.ToBeDeployedAreaConsumption.Name, 
                CanOverrideWithResult.ResultingAreaConsumption.Name);
    }
}