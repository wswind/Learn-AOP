using Castle.DynamicProxy;
using CommonLib.Aop;
using System.IO;
using System.Threading.Tasks;

//https://stackoverflow.com/a/39784559/7726468
//need Microsoft.CSharp package for .net standard projects
namespace AutofacAsyncInterceptor
{
    public class CallLoggerAsyncInterceptor : InterceptorBase
    {
        TextWriter _output;

        public CallLoggerAsyncInterceptor(TextWriter output)
        {
            _output = output;
        }

        protected override void InterceptSynchronous(IInvocation invocation)
        {
            _output.WriteLine("InterceptSynchronous Before");
            invocation.Proceed();
            _output.WriteLine("InterceptSynchronous After");
        }

        protected override async Task InterceptAsync(Task task)
        {
            _output.WriteLine("Task InterceptAsync Before");
            await task.ConfigureAwait(false);
            _output.WriteLine("Task InterceptAsync After");
        }

        protected override async Task<T> InterceptAsync<T>(Task<T> task)
        {
            _output.WriteLine("Task<T> InterceptAsync Before Await");//error: the task runs eariler than this line
            T result = await task.ConfigureAwait(false);
            _output.WriteLine("Task<T> InterceptAsync After Await, Result is '{0}'.", result.ToString());
            result = (T)(object)"changed result";
            return result;
        }

    }
}
