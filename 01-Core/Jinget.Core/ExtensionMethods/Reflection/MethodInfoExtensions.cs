using Microsoft.VisualStudio.Threading;
using System.Reflection;

namespace Jinget.Core.ExtensionMethods.Reflection
{
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Dynamically invoke async method
        /// </summary>
        public static object InvokeAsync(this MethodInfo method, object obj, params object[] parameters)
        {
            using var taskContext = new JoinableTaskContext();
            var joinableTaskFactory = new JoinableTaskFactory(taskContext);
            return joinableTaskFactory.Run(async () =>
            {
                dynamic awaitable = method.Invoke(obj, parameters);
                await awaitable;
                return awaitable.GetAwaiter().GetResult();
            });
        }
    }
}
