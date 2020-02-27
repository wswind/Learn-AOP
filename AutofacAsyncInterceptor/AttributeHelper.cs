using System.Linq;
using System.Reflection;

namespace AutofacAsyncInterceptor
{
    //use MemberInfo or MethodInfo
    public static class AttributeHelper
    {
        public static bool IsLoggerEnabled(MethodInfo type)
        {
            return GetStartLog(type);
        }
        public static bool HasCustomAttribute(MemberInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(CustomAttribute), true);
        }

        private static bool GetStartLog(MethodInfo methodInfo)
        {
            var attrs = methodInfo.GetCustomAttributes(true).OfType<CustomAttribute>().ToArray();
            if (attrs.Any())
            {
                CustomAttribute customAttribute = attrs.First();
                return customAttribute.StartLog;
            }
            return false;
        }
    }
}
