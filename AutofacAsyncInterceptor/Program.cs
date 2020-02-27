/*
 https://autofac.readthedocs.io/en/latest/advanced/interceptors.html
 this is a demo for autofac interceptors
 */


using Autofac;
using Autofac.Extras.DynamicProxy;
using System;

namespace AutofacInterceptor
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

            builder.RegisterType<CallLogger>();

            // Typed registration
            builder.Register(c => new CallLoggerAsync(Console.Out));

            //// Named registration
            //builder.Register(c => new CallLogger(Console.Out))
            //       .Named<IInterceptor>("log-calls");


            // Enable Interception on Types
            //builder.RegisterType<SomeType>()
            //       .As<ISomeType>()
            //       .EnableInterfaceInterceptors()
            //       .InterceptedBy(typeof(CallLogger));


            //var type = typeof(SomeType);
            //var typeInfo = type.GetTypeInfo();
            //var b = LoggerHelper.IsLoggerEnabled(typeInfo);
           
            var container = builder.Build();
            var willBeIntercepted = container.Resolve<ISomeType>();
            willBeIntercepted.Show("this is a test");
        }
    }
}
