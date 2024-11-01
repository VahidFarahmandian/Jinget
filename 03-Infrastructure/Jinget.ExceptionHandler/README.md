# Jinget ExceptionHandler
Using this library, you can make use of different exception handling mechanisms.

## How to Use:

Download the package from NuGet using Package Manager:
`Install-Package Jinget.ExceptionHandler`
You can also use other methods supported by NuGet. Check [Here](https://www.nuget.org/packages/Jinget.ExceptionHandler "Here") for more information.

## Configuration

Call `builder.Services.ConfigureJingetExceptionHandler()` to add required services to DI container. Calling this method requires to pass an object of `BaseSettingModel` which is consists of following properties:

`UseGlobalExceptionHandler`: If set to true then global exception handler will be used which in turn will rewrite the exception response output.

`Handle4xxResponses`: If set to true then http request exception handler will be used which in turn will handle the 4xx responses.

After registering the required services, you need to call `app.UseJingetExceptionHandler()` method to add exception handler middleware to your request pipeline.

****Note****: If you are using `Jinget.Logger` then you do not need to do anything and exception handling is by default set by this package.

---
## How to install
In order to install Jinget ExceptionHandler please refer to [nuget.org](https://www.nuget.org/packages/Jinget.ExceptionHandler "nuget.org")

## Contact Me
👨‍💻 Twitter: https://twitter.com/_jinget

📧 Email: farahmandian2011@gmail.com

📣 Instagram: https://www.instagram.com/vahidfarahmandian
