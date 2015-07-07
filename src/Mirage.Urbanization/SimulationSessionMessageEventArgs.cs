using System;

namespace Mirage.Urbanization
{
    public class SimulationSessionMessageEventArgs : EventArgs
    {
        private readonly string _message;

        public SimulationSessionMessageEventArgs(string message)
        {
            _message = message;
        }

        public string Message { get { return _message; } }
    }
}