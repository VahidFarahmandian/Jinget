using Jinget.Logger.Entities.Log;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jinget.Logger.ViewModels
{
    internal record LogViewModel(IGrouping<Guid, OperationLog> Op, IReadOnlyCollection<ErrorLog> Err);
}