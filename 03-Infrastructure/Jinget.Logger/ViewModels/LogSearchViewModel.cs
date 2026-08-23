using Jinget.Logger.Enum;

namespace Jinget.Logger.ViewModels;

public record LogSearchViewModel(
    string TraceIdentifier,
    string? RequestUrl,
    string? Method,
    string? SubSystem,

    // number of logs
    int HopCount,

    // Wall-clock duration: EndTime - StartTime.
    double TraceDurationMilliseconds,

    long TotalContentLength,
    DateTime StartTime,
    DateTime EndTime,
    TraceFinalStatus FinalStatus,
    IReadOnlyList<LogModel> Logs);