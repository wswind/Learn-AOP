using System;

namespace ProxyPattern
{
    public class BusinessModuleProxy : IBusinessModule
    {
        BusinessModule _realObject;
        public BusinessModuleProxy()
        {
            _realObject = new BusinessModule();
        }
        public void Method1()
        {
            Console.WriteLine("BusinessModuleProxy before");
            _realObject.Method1();
            Console.WriteLine("BusinessModuleProxy after");
        }
    }
}
