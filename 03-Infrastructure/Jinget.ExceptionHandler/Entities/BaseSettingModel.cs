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

    const long MB_10 = 1024 * 1024 * 10;

    /// <summary>
    /// maximum request body size to log. 
    /// request body larger than this value will be logged as `--REQUEST BODY TOO LARGE--` string
    /// </summary>
    public long MaxRequestBodySize { get; set; } = MB_10;

    /// <summary>
    /// maximum response body size to log. 
    /// response body larger than this value will be logged as `--REQUEST BODY TOO LARGE--` string
    /// </summary>
    public long MaxResponseBodySize { get; set; } = MB_10;
}
