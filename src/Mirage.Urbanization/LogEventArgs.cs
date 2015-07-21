using System;

namespace Mirage.Urbanization
{
    public class LogEventArgs : EventArgs
    {
        public string LogMessage { get; }
        public LogEventArgs(string logMessage)
        {
            LogMessage = logMessage;
        }
    }
}