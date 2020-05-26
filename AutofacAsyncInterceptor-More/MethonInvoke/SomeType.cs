using System;
using System.Threading.Tasks;

namespace AutofacAsyncInterceptor
{
    public class SomeType : ISomeType
    {
        public async Task<string> Show(string input)
        {
            Console.WriteLine($"showdemo");
            return await Task.FromResult("resultdemo");
        }
    }
}
