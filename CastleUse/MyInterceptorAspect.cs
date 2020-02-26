using System;
using Castle.DynamicProxy;

namespace CastleUse
{
public class MyInterceptorAspect : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        Console.WriteLine("Interceptor before");
        invocation.Proceed();
        Console.WriteLine("Interceptor after");
    }
}
}