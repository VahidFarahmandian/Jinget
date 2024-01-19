namespace Jinget.Core.DiScanner.Contracts;

public interface ISelector
{
    void Populate(IServiceCollection services, RegistrationStrategy? options);
}
