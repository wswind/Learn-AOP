using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutofacAsyncInterceptor
{
    public class SomeType : ISomeType
    {
        public Task<string> Show(string input)
        {
            //Console.WriteLine("before run");
            //await Task.Delay(2000);
            //Console.WriteLine("after run");
            //return "shows now";
            return Task.Factory.StartNew<string>( () =>
                {
                    Console.WriteLine("SomeType - Show Before Sleep");
                    Thread.Sleep(2000);
                    Console.WriteLine("SomeType - Show After Sleep");
                    return "some type shows";
                });
        }

    }
}
