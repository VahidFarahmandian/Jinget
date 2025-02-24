namespace Jinget.Logger.ViewModels;

public record LogSearchViewModel(string? TraceIdentifier, IEnumerable<LogModel> Logs);