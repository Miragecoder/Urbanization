using System;
using System.Collections;
using System.Collections.Generic;
using Mirage.Urbanization.ZoneConsumption;
using Mirage.Urbanization.ZoneConsumption.Base;

namespace Mirage.Urbanization
{
    public interface IAreaMessage
    {
        string Message { get; }
    }

    public interface IAreaConsumptionResult : IAreaMessage
    {
        bool Success { get; }
        IAreaConsumption AreaConsumption { get; }
    }

    public class SimulationSessionMessageEventArgs : EventArgs
    {
        private readonly string _message;

        public SimulationSessionMessageEventArgs(string message)
        {
            _message = message;
        }

        public string Message { get { return _message; } }
    }

    public class AreaConsumptionResultEventArgs : EventArgs
    {
        private readonly IAreaConsumptionResult _areaConsumptionResult;
        public IAreaConsumptionResult AreaConsumptionResult { get { return _areaConsumptionResult; } }
        public AreaConsumptionResultEventArgs(IAreaConsumptionResult areaConsumptionResult)
        {
            if (areaConsumptionResult == null) throw new ArgumentNullException("areaConsumptionResult");
            _areaConsumptionResult = areaConsumptionResult;
        }
    }

    internal class AreaConsumptionResult : IAreaConsumptionResult
    {
        private readonly IAreaConsumption _areaConsumption;
        private readonly string _message;
        private readonly bool _success;

        public bool Success { get { return _success; } }
        public string Message { get { return _message; } }
        public IAreaConsumption AreaConsumption { get { return _areaConsumption; } }

        public AreaConsumptionResult(IAreaConsumption areaConsumption, bool success, string message)
        {
            _areaConsumption = areaConsumption;
            _message = message;
            _success = success;
        }
    }
}