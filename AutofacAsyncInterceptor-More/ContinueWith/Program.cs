/*
 https://autofac.readthedocs.io/en/latest/advanced/interceptors.html
 this is a demo for autofac interceptors
 */


using Autofac;
using Autofac.Extras.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace AutofacAsyncInterceptor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // create builder
            var builder = new ContainerBuilder();

            builder.RegisterType<SomeType>()
              .As<ISomeType>()
              .EnableInterfaceInterceptors();

            builder.Register(c => new CallLoggerAsyncInterceptor(Console.Out));

            var container = builder.Build();
            var willBeIntercepted = container.Resolve<ISomeType>();
            Console.WriteLine("main - before call show");
            var val = await willBeIntercepted.Show("this is a test");
            Console.WriteLine("main - after call show");
            Console.WriteLine($"main return value is {val}");
        }

     
    }
}
