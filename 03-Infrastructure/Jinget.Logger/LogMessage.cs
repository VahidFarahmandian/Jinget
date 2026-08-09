namespace Jinget.Logger;

public class LogMessage
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public Microsoft.Extensions.Logging.LogLevel Severity { get; set; }
    public string? Exception { get; set; }

    /// <summary>
    /// The logging category that produced this message.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Identifies the event associated with this log message.
    /// </summary>
    public EventId EventId { get; set; }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append(Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"));
        builder.Append(" [");
        builder.Append(Severity);
        builder.Append("] ");

        if (!string.IsNullOrWhiteSpace(Category))
        {
            builder.Append("[");
            builder.Append(Category);
            builder.Append("] ");
        }

        builder.AppendLine(Description);
        builder.AppendLine(Exception);

        builder.Append(Environment.NewLine);

        return builder.ToString();
    }
}