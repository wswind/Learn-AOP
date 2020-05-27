using Castle.DynamicProxy;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

//https://github.com/castleproject/Core/issues/107
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

            //if (IsEnabled(invocation))
            //{
            //    _output.WriteLine("Before Invocation");
            //}
            Console.WriteLine("before procced");
            invocation.Proceed();
            //Thread.Sleep(3000);
            Console.WriteLine("after procced");
            var returnValue = (Task<string>)invocation.ReturnValue;

            returnValue.ContinueWith(t =>
            {
                _output.WriteLine("before continue with:" + t.Result);
                return "changed value";
            });

            //var ret = returnValue.Result;
            //invocation.ReturnValue = Task.FromResult("changed value");
            Console.WriteLine("Invocation ends");
        }

        private bool IsEnabled(IInvocation invocation)
        {
            bool isEnabled = AttributeHelper.IsLoggerEnabled(invocation.Method);
            return isEnabled;
        }
    }
}
