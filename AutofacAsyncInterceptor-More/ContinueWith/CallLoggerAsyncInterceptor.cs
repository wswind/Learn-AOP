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
            Console.WriteLine("after procced");

#if RUN_IT_NOW
            Console.WriteLine("returnValue.Result Before");
            var ret = returnValue.Result;
            Console.WriteLine("returnValue.Result End");
            invocation.ReturnValue = Task.FromResult("changed value");
#else 
            var returnValue = (Task<string>)invocation.ReturnValue;
            returnValue.ContinueWith(t =>
            {
                _output.WriteLine("continue with:" + t.Result);
                return t;
            });
#endif
            Console.WriteLine("Invocation ends");
        }

        private bool IsEnabled(IInvocation invocation)
        {
            bool isEnabled = AttributeHelper.IsLoggerEnabled(invocation.Method);
            return isEnabled;
        }
    }
}
