namespace Mirage.Urbanization
open System
type LogEventArgs(logMessage) =
   inherit EventArgs() 
   member val LogMessage = logMessage with get
   member val CreatedOn = DateTime.Now with get
