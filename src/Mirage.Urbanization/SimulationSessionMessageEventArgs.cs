using System;

namespace Mirage.Urbanization
{
    public class SimulationSessionMessageEventArgs : EventArgs
    {
        public SimulationSessionMessageEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}