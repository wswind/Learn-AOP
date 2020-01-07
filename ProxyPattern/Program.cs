namespace ProxyPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            IBusinessModule module = new BusinessModuleProxy();
            module.Method1();
        }
    }
}
