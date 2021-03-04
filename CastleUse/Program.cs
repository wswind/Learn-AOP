using Castle.DynamicProxy;

namespace CastleUse
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxyGenerator = new ProxyGenerator();
            //the intercepted function must be virtual
            var svc = proxyGenerator.CreateClassProxy<MessageClient>(new MyInterceptorAspect());
            svc.Send("hi");
            
            var svc2 = proxyGenerator.CreateInterfaceProxyWithTarget<IHelloClient>(new HelloClient(), new MyInterceptorAspect());
            //difference between CreateInterfaceProxyWithTargetInterface see:
            //https://kozmic.net/2009/11/13/interfaceproxywithtarget-interfaceproxywithtargetinterface-ndash-whatrsquos-the-difference/
            svc2.Hello();
        }
    }
}
