using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{

    //[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomAttribute : Attribute
    {
        public bool StartLog { get; set; }
    }
}
