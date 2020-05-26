using Castle.DynamicProxy;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

//https://stackoverflow.com/a/39784559/7726468
namespace AutofacAsyncInterceptor
{
    public class CallLoggerAsyncInterceptor : IInterceptor  
    {
        TextWriter _output;

        public CallLoggerAsyncInterceptor(TextWriter output)
        {
            _output = output;
        }

        public void Intercept(IInvocation invocation)
        {
            _output.WriteLine("Calling method '{0}' with parameters '{1}'... ",
                 invocation.Method.Name,
                 string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray()));

            if (IsEnabled(invocation))
            {
                _output.WriteLine("Before Invocation");
            }

            invocation.Proceed();
            

            var method = invocation.MethodInvocationTarget;
            var isAsync = method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
            if (isAsync && typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                invocation.ReturnValue = InterceptAsync((dynamic)invocation.ReturnValue);
            }
        }
        private async Task InterceptAsync(Task task)
        {
            await task.ConfigureAwait(false);
            // do the continuation work for Task...
        }

        private async Task<T> InterceptAsync<T>(Task<T> task)
        {
            T result = await task.ConfigureAwait(false);
            // do the continuation work for Task<T>...
            _output.WriteLine("After Invocation, Result is '{0}'.", result.ToString());
            return result;
        }

        private async Task<T> HandleAsyncWithResult<T>(Task<T> task, IInvocation invocation)
        {
            var result = await task;
            if (IsEnabled(invocation))
            {
                _output.WriteLine("After Invocation, Result is '{0}'.", result);
            }
            return result;
        }

        private bool IsEnabled(IInvocation invocation)
        {
            bool isEnabled = AttributeHelper.IsLoggerEnabled(invocation.Method);
            return isEnabled;
        }
    }
}
