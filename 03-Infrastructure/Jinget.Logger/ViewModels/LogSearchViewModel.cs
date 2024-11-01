using Jinget.ExceptionHandler.Entities.Log;

namespace Jinget.Logger.ViewModels;

// public record LogSearchViewModel(Guid RequestId, IEnumerable<LogModel> Logs);
public record LogSearchViewModel(string TraceIdentifier, IEnumerable<LogModel> Logs);