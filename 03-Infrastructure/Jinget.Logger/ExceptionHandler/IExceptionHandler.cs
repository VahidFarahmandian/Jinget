namespace Jinget.Logger.ExceptionHandler;

public interface IExceptionHandler<TCategoryName>
{
    void Handle(Exception ex, object details);
}