/*
 https://autofac.readthedocs.io/en/latest/advanced/interceptors.html
 this is a demo for autofac interceptors
 */


using Autofac;
using Autofac.Extras.DynamicProxy;
using AutofacInterceptor;
using System;

namespace AutofacAsyncInterceptor
{

    class Program
    {
        static void Main(string[] args)
        {
            // create builder
            var builder = new ContainerBuilder();

            builder.RegisterType<SomeType>()
              .As<ISomeType>()
              .EnableInterfaceInterceptors();
             
            //register adapter
            builder.RegisterType<CallLogger>();
            //register async interceptor
            builder.Register(c => new CallLoggerAsync(Console.Out));

            var container = builder.Build();
            var willBeIntercepted = container.Resolve<ISomeType>();
            willBeIntercepted.Show("this is a test");
        }
    }
}
