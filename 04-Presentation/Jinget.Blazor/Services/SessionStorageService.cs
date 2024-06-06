namespace Jinget.Blazor.Services;

public class SessionStorageService(IJSRuntime js) : BrowserStorageService(js, "sessionStorage"), ISessionStorageService
{
}