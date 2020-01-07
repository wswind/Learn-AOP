using System;

public class TwitterClient
{
    public virtual void Send(string msg)
    {
        Console.WriteLine("Sending: {0}", msg);
    }
}