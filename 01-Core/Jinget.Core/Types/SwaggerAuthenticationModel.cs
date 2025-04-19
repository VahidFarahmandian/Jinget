namespace Jinget.Core.Types;

public class SwaggerAuthenticationModel
{
    public string ApplicationTitle { get; set; }
    public string Version { get; set; } = "v1";
    public string SchemeName { get; set; } = "Bearer";
    public string SchemeDescription { get; set; } = "JWT Authorization header using the Bearer scheme. Example: \"jinget.token: Bearer {token}\"";
}