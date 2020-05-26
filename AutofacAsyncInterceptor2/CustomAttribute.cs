using System;

namespace AutofacAsyncInterceptor
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CustomAttribute : Attribute
    {
        public bool StartLog { get; set; }
    }
}
