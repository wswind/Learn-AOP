using Autofac.Extras.DynamicProxy;
using System.Threading.Tasks;

namespace AutofacAsyncInterceptor
{
    public interface ISomeType
    {
        Task<string> ShowAsync(string input);
        Task ShowAsync2(string input);
        void ShowSynchronous(string input);
    }
}
