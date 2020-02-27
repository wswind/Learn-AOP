using Autofac.Extras.DynamicProxy;
using System.Threading.Tasks;

namespace AutofacInterceptor
{
    [Intercept(typeof(CallLogger))]
    public interface ISomeType
    {
        [Custom(StartLog = true)]
        Task<string> Show(string input);
    }
}
