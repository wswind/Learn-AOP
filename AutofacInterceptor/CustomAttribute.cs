using System;

namespace AutofacInterceptor
{

    [AttributeUsage(AttributeTargets.Method)]
    public class CustomAttribute : Attribute
    {
        public bool StartLog { get; set; }
    }
}
