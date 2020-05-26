using Autofac.Extras.DynamicProxy;
using System.Threading.Tasks;

namespace AutofacAsyncInterceptor
{
    [Intercept(typeof(CallLoggerAsyncInterceptor))]
    public interface ISomeType
    {
        [Custom(StartLog = true)]
        Task<string> Show(string input);
    }
}
