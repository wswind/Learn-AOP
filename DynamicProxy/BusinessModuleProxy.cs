using System;

namespace DynamicProxy
{
    //这个类在运行时其实没有使用
    public class BusinessModuleProxy
    {
        BusinessModule _realObject;

        public BusinessModuleProxy(BusinessModule svc)
        {
            _realObject = svc;
        }
        public void Method1(string message)
        {
            Console.WriteLine("Method1 before!");
            _realObject.Method1(message);
            Console.WriteLine("Method1 after!");
        }
    }
}