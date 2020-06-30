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
        async static Task Main(string[] args)
        {
            // create builder
            var builder = new ContainerBuilder();

            builder.RegisterType<SomeType>()
              .As<ISomeType>()
              .EnableInterfaceInterceptors()
              .InterceptedBy(typeof(CallLoggerAsyncInterceptor));
             
            //register async interceptor
            builder.Register(c => new CallLoggerAsyncInterceptor(Console.Out));

            var container = builder.Build();
            var willBeIntercepted = container.Resolve<ISomeType>();
            var result = await willBeIntercepted.ShowAsync("this is a test");
            Console.WriteLine($"ShowAsync Return Value Is {result}");
            await willBeIntercepted.ShowAsync2("this is a test");
            willBeIntercepted.ShowSynchronous("this is a test");
        }
    }
}
