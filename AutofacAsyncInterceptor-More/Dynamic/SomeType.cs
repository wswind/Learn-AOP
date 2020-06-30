using System;
using System.Threading.Tasks;

namespace AutofacAsyncInterceptor
{
    public class SomeType : ISomeType
    {
        public async Task<string> ShowAsync(string input)
        {
            Console.WriteLine("Task<string> ShowAsync(string input) Before Await");
            await Task.Delay(1000);
            Console.WriteLine("Task<string> ShowAsync(string input) After Await");
            return "some type shows";//change return value
        }

        public async Task ShowAsync2(string input)
        {
            Console.WriteLine("Task ShowAsync(string input) Before Await");
            await Task.Delay(1000);
            Console.WriteLine("Task ShowAsync(string input) After Await");
        }

        public void ShowSynchronous(string input)
        {
            Console.WriteLine("ShowSynchronous");
        }

    
    }
}
