using Microsoft.VisualStudio.Threading;
using System.Reflection;

namespace Jinget.Core.ExtensionMethods.Reflection
{
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Dynamically invoke async method
        /// </summary>
#pragma warning disable VSTHRD200 // Use "Async" suffix for async methods
        public static object InvokeAsync(this MethodInfo method, object obj, params object[] parameters)
#pragma warning restore VSTHRD200 // Use "Async" suffix for async methods
        {
            using var taskContext = new JoinableTaskContext();
            var joinableTaskFactory = new JoinableTaskFactory(taskContext);
            return joinableTaskFactory.Run(async () =>
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                dynamic awaitable = method.Invoke(obj, parameters);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                await awaitable;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                return awaitable.GetAwaiter().GetResult();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            });
        }
    }
}
