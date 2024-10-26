using Jinget.Blazor.Extensions;
using Jinget.Blazor.Server.Test.Components;
using Jinget.Blazor.Services.Contracts;
using Jinget.Blazor.Test;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddJingetBlazor(tokenConfigModel:
    new Jinget.Blazor.Models.TokenConfigModel
    {
        TokenName = "token",
    });
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
var app = builder.Build();
app.UsePathBase("/Jinget");

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
