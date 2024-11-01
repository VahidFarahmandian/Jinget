namespace Jinget.ExceptionHandler.Entities;

public class BaseSettingModel
{
    /// <summary>
    /// if set to true then Global exception handler will be used which in turn will be rewrite the exception response output
    /// </summary>
    public bool UseGlobalExceptionHandler { get; set; } = true;

    /// <summary>
    /// if set to true then http request exception handler will be used which in turn will be handle the 4xx responses
    /// </summary>
    public bool Handle4xxResponses { get; set; } = false;
}
