using Castle.DynamicProxy;

namespace CastleUse
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxyGenerator = new ProxyGenerator();
            var svc = proxyGenerator.CreateClassProxy<MessageClient>(new MyInterceptorAspect());
            svc.Send("hi");

            var svc2 = proxyGenerator.CreateInterfaceProxyWithTarget<IHelloClient>(new HelloClient(), new MyInterceptorAspect());
            svc2.Hello();
        }
    }
}
