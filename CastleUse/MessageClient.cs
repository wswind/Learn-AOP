using System;

namespace CastleUse
{
    public class MessageClient
    {
        public virtual void Send(string msg)
        {
            Console.WriteLine("Sending: {0}", msg);
        }
    }
}
