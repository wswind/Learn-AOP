using System;
using System.Threading.Tasks;

namespace AutofacInterceptor
{
    public class SomeType : ISomeType
    {
        public Task<string> Show(string input)
        {
            Console.WriteLine($"showdemo");
            return Task.FromResult("resultdemo");
        }
    }
}
