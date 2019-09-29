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
            throw new NotImplementedException();
        }
    }
}
