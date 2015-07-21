using System;

namespace Mirage.Urbanization
{
    public class LogEventArgs : EventArgs
    {
        public string LogMessage { get; }
        public DateTime CreatedOn { get; } = DateTime.Now;
        public LogEventArgs(string logMessage)
        {
            LogMessage = logMessage;
        }
    }
}