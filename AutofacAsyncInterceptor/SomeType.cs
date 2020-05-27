using System;
using System.Threading.Tasks;

namespace AutofacAsyncInterceptor
{
    public class SomeType : ISomeType
    {
        public Task<string> Show(string input)
        {
            return Task.Factory.StartNew<string>(() =>
            {
                Console.WriteLine("SomeType Show() Runs");
                return "some type shows";
            });
        }
    }
}
