namespace Jinget.Logger;

public class LogMessage
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string? Description { get; set; }
    public Microsoft.Extensions.Logging.LogLevel Severity { get; set; }
    public string? Exception { get; set; }

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.Append(Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"));
        builder.Append(" [");
        builder.Append(Severity);
        builder.Append("] ");
        builder.AppendLine(Description);
        builder.AppendLine(Exception);

        builder.Append(Environment.NewLine);

        return builder.ToString();
    }
}