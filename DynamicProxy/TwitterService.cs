using System;

public interface ITwitterService
{
    void Tweet(string message);
}
public class MyTwitterService : ITwitterService
{
    public void Tweet(string message)
    {
        Console.WriteLine("Tweeting: {0}", message);
    }
}

public class MyTwitterServiceProxy
{
    MyTwitterService _realObject;

    public MyTwitterServiceProxy(MyTwitterService svc)
    {
        _realObject = svc;
    }
    public void Tweet(string message)
    {
        Console.WriteLine("Hello before!");
        _realObject.Tweet(message);
        Console.WriteLine("Hello after!");
    }
}