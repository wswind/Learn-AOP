using System;
using Castle.DynamicProxy;

public class MyInterceptorAspect : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        Console.WriteLine("Interceptor 1");
        invocation.Proceed();
        Console.WriteLine("Interceptor 2");
    }
}