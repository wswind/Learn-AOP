using System;
using System.Threading.Tasks;

namespace AutofacAsyncInterceptor
{
    public class SomeType : ISomeType
    {
        public async Task<string> Show(string input)
        {
            Console.WriteLine("Before Await");
            await Task.Delay(1000);
            Console.WriteLine("After Await");
            return "some type shows";
        }
    }
}
