using Castle.DynamicProxy;
using System.IO;
using System.Linq;

namespace AutofacInterceptor
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

            //校验方法是否需要开启了Logger
            bool isEnabled = AttributeHelper.IsLoggerEnabled(invocation.Method);

            //方法执行前
            if (isEnabled)
            {
                _output.WriteLine("Logger is Enabled");
            }
            //被拦截的方法执行
            invocation.Proceed();

            //方法执行后
            if (isEnabled)
            {
                _output.WriteLine("Done: result was '{0}'.", invocation.ReturnValue);
            }
        }
    }
}
