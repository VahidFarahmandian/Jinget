using System.ComponentModel;

namespace Jinget.Logger.Enums;

public enum LogType : byte
{
    [Description("request")] Request = 1,
    [Description("response")] Response = 2,
    [Description("error")] Error = 3
}