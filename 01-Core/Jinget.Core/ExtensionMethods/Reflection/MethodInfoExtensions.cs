namespace Jinget.Core.ExtensionMethods.Reflection;

public static class MethodInfoExtensions
{
    /// <summary>
    /// Dynamically invoke async method
    /// </summary>
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
    public static object? InvokeAsync(this MethodInfo method, object obj, params object[] parameters)
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
    {
        using var taskContext = new JoinableTaskContext();
        var joinableTaskFactory = new JoinableTaskFactory(taskContext);
        return joinableTaskFactory.Run(async () =>
        {
            dynamic? awaitable = method.Invoke(obj, parameters);
            await awaitable;

            return awaitable?.GetAwaiter().GetResult();
        });
    }
}
