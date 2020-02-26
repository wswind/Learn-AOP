using Autofac.Extras.DynamicProxy;

namespace AutofacInterceptor
{
[Intercept(typeof(CallLogger))] 
public interface ISomeType
{
    [Custom(StartLog = true)]
    string Show(string input);
}
}
