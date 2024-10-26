namespace Jinget.Logger.ViewModels;

public record LogSearchViewModel(Guid RequestId, IEnumerable<LogModel> Logs);