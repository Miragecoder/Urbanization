namespace Mirage.Urbanization
open System

type LogEventArgs(logMessage) =
   inherit EventArgs() 
   member val LogMessage = logMessage with get
   member val CreatedOn = DateTime.Now with get

type IAreaMessage =
   abstract member Message : string with get

type BuildStyle = SingleClick = 0 | ClickAndDrag = 1

type IAreaConsumption =
    abstract member Name : string with get
    abstract member KeyChar : char with get
    abstract member Cost : int with get
    abstract member BuildStyle : BuildStyle with get

type IAreaConsumptionResult =
    inherit IAreaMessage
    abstract member Success : bool with get
    abstract member AreaConsumption : IAreaConsumption with get

