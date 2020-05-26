using System.Linq;
using System.Reflection;

namespace AutofacAsyncInterceptor
{
    //use MemberInfo or MethodInfo
    public static class AttributeHelper
    {
        public static bool IsLoggerEnabled(MethodInfo mi)
        {
            var attrs = mi.GetCustomAttributes(true).OfType<CustomAttribute>().ToArray();
            if (attrs.Any())
            {
                CustomAttribute customAttribute = attrs.First();
                return customAttribute.StartLog;
            }
            return false;
        }
        public static bool HasCustomAttribute(MemberInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(CustomAttribute), true);
        }

      
    }
}
