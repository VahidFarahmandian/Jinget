namespace Jinget.Blazor.Services;

public class LocalStorageService(IJSRuntime js) : BrowserStorageService(js, "localStorage"), ILocalStorageService
{
}