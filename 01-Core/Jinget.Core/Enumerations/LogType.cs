namespace Jinget.Core.Enumerations;

public enum LogType : byte
{
    [Description("request")] Request = 1,
    [Description("response")] Response = 2,
    [Description("customlog")] CustomLog = 3,
    [Description("error")] Error = 4
}