using System;

namespace ProxyPattern
{
    public interface IBusinessModule
    {
        void Method1();
    }

    public class BusinessModule : IBusinessModule
    {
        public void Method1()
        {
            Console.WriteLine("Method1");
        }
    }
}


