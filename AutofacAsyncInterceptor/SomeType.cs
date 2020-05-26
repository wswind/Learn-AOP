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
                Console.WriteLine("run task");
                return "some type shows";
            });
        }
    }
}
