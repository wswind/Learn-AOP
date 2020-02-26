using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    public class CallLogger : IInterceptor
    {
        TextWriter _output;

        public CallLogger(TextWriter output)
        {
            _output = output;
        }

        public void Intercept(IInvocation invocation)
        {
            _output.WriteLine("Calling method '{0}' with parameters '{1}'... ",
              invocation.Method.Name,
              string.Join(", ", invocation.Arguments.Select(a => (a ?? "").ToString()).ToArray()));

            if(LoggerHelper.IsLoggerEnabled(invocation.Method))
            {
                _output.WriteLine("Logger is Enabled");
            }

            invocation.Proceed();

            _output.WriteLine("Done: result was '{0}'.", invocation.ReturnValue);
        }
    }
}
