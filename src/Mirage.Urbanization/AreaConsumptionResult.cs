using System;
using System.Collections;
using System.Collections.Generic;
using Mirage.Urbanization.ZoneConsumption;

namespace Mirage.Urbanization
{
    public interface IAreaMessage
    {
        string Message { get; }
    }

    public interface IAreaConsumptionResult : IAreaMessage
    {
        bool Success { get; }
    }

    public class AreaMessageEventArgs : EventArgs
    {
        private readonly IAreaMessage _message;
        public IAreaMessage Message { get { return _message; } }
        public AreaMessageEventArgs(IAreaMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            _message = message;
        }
    }

    internal class AreaConsumptionResult : IAreaConsumptionResult
    {
        private readonly string _message;
        private readonly bool _success;

        public bool Success { get { return _success; } }
        public string Message { get { return _message; } }

        public AreaConsumptionResult(string message, bool success)
        {
            _message = message;
            _success = success;
        }
    }
}