using Autofac.Extras.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    [Intercept(typeof(CallLogger))] 
    public interface ISomeType
    {
        [Custom(StartLog = true)]
        string Show(string input);
    }
}
