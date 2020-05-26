using Castle.DynamicProxy;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

//https://stackoverflow.com/a/28374134/7726468
namespace AutofacAsyncInterceptor
{
    public class CallLoggerAsyncInterceptor : IInterceptor  
    {
        TextWriter _output;

        public CallLoggerAsyncInterceptor(TextWriter output)
        {
            _output = output;
        }

        private static readonly MethodInfo HandleAsyncWithResultMethodInfo = typeof(CallLoggerAsyncInterceptor).GetMethod(nameof(HandleAsyncWithResult), BindingFlags.Instance | BindingFlags.NonPublic);

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

            var taskType = invocation.Method.ReturnType;//warnning:this line with make the task run
            var resultType = taskType.GetGenericArguments()[0];
            var mi = HandleAsyncWithResultMethodInfo.MakeGenericMethod(resultType);
            invocation.ReturnValue = mi.Invoke(this, new[] { invocation.ReturnValue, invocation });
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
