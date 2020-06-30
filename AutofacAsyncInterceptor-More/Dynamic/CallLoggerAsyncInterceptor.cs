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
            var method = invocation.MethodInvocationTarget;
            var isAsync = method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
            if (isAsync && typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                invocation.Proceed();
                invocation.ReturnValue = InterceptAsync((dynamic)invocation.ReturnValue);
            }
            else 
            {
                InterceptSynchronous(invocation);
            }
            Console.WriteLine("Intercept Ends");
        }

        private void InterceptSynchronous(IInvocation invocation)
        {
            _output.WriteLine("InterceptSynchronous Before");
            invocation.Proceed();
            _output.WriteLine("InterceptSynchronous After");
        }

        private async Task InterceptAsync(Task task)
        {
            _output.WriteLine("Task InterceptAsync Before");
            await task.ConfigureAwait(false);
            _output.WriteLine("Task InterceptAsync After");
        }

        private async Task<T> InterceptAsync<T>(Task<T> task)
        {
            _output.WriteLine("Task<T> InterceptAsync Before Await");//error: the task runs eariler than this line
            T result = await task.ConfigureAwait(false);
            _output.WriteLine("Task<T> InterceptAsync After Await, Result is '{0}'.", result.ToString());
            result = (T)(object)"changed result";
            return result;
        }

    }
}
