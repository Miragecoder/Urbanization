using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mirage.Urbanization
{
    public class Logger
    {
        public static Logger Instance { get; } = new Logger();
        public void WriteLine(object @object)
        {
            OnLogMessage?.Invoke(this, new LogEventArgs(@object.ToString()));
        }

        public event EventHandler<LogEventArgs> OnLogMessage;

        const string EventSource = "Urbanization";
        public void LogException(Exception exception, string operationDescription, int eventId)
        {
            if (!EventLog.SourceExists(EventSource))
                EventLog.CreateEventSource(EventSource, "Application");
            
            EventLog.WriteEntry(
                source: EventSource, 
                message: "An unhandled exception was thrown during an operation with the "
                + $"following description: '{operationDescription}'. Exception details are: {exception} ", 
                type: EventLogEntryType.Error, 
                eventID: eventId);
        }
    }
}
