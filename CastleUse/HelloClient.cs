using System;

public class HelloClient : IHelloClient
{
    public void Hello()
    {
        Console.WriteLine("Hello");
    }
}

public interface IHelloClient
{
    void Hello();
}