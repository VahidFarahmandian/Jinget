using System;
using System.Collections.Generic;
using static Jinget.Logger.ViewModels.LogSearchViewModel;

namespace Jinget.Logger.ViewModels;

public record LogSearchViewModel(
    Guid RequestId,
    string IP,
    string Url,
    string Method,
    string PageUrl,
    IEnumerable<OperationLogSearchViewModel> Operations,
    IEnumerable<ErrorLogSearchViewModel> Errors)
{
    public record OperationLogSearchViewModel(
        string Type,
        DateTime When,
        string Body,
        long ContentLength,
        string Headers,
        string Detail,
        string Description);

    public record ErrorLogSearchViewModel(
        DateTime When,
        string Description,
        string Severity);
}