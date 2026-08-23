using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jinget.Logger.Enum;

public enum TraceFinalStatus : byte
{
    AwaitingResponse,
    Success,
    Warning,
    Failed
}
