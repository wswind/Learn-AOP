using Castle.DynamicProxy;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
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
            Console.WriteLine("Intercept Begins");
            _output.WriteLine("Calling method '{0}' with parameters '{1}'... ",
                 invocation.Method.Name,
                 string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray()));

            Console.WriteLine("before proceed");
            invocation.Proceed();
            Console.WriteLine("after proceed");
            Thread.Sleep(2000);
            invocation.ReturnValue = InterceptAsync((dynamic)invocation.ReturnValue);
            Console.WriteLine("Intercept Ends");
        }
        private async Task InterceptAsync(Task task)
        {
            await task.ConfigureAwait(false);
            // do the continuation work for Task...
        }

        private async Task<T> InterceptAsync<T>(Task<T> task)
        {
            Console.WriteLine("InterceptAsync<T> Before Await");//error: the task runs eariler than this line
            T result = await task.ConfigureAwait(false);
            // do the continuation work for Task<T>...
            Console.WriteLine("InterceptAsync<T> After Await, Result is '{0}'.", result.ToString());
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
