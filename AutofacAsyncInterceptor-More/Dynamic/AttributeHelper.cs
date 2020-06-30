using System.Linq;
using System.Reflection;

namespace CommonLib.Aop
{
    public static class AttributeHelper
    {
        public static T GetCustomAttribute<T>(MethodInfo mi) where T : class
        {
            var attrs = mi.GetCustomAttributes(true).OfType<T>().ToArray();
            if (attrs.Any())
            {
                T customAttribute = attrs.First();
                return customAttribute;
            }
            else 
                return null;
        }
    }
   
}
