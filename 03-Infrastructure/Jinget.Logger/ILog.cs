namespace Jinget.Logger;

public interface ILog
{
    Task LogAsync(HttpContext context);
}