using System;

namespace DynamicProxy
{
    public interface IBusinessModule
    {
        void Method1(string message);
    }
    public class BusinessModule : IBusinessModule
    {
        public void Method1(string message)
        {
            Console.WriteLine("Method1: {0}", message);
        }
    }
}