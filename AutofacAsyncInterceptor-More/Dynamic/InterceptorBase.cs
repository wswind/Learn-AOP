using Castle.DynamicProxy;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CommonLib.Aop
{
    //https://stackoverflow.com/a/39784559/7726468
    public abstract class InterceptorBase : IInterceptor
    {
        public InterceptorBase()
        { 
        }
        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget;
            if (IsAsyncMethod(method))
            {
                invocation.Proceed();
                invocation.ReturnValue = InterceptAsync((dynamic)invocation.ReturnValue);
            }
            else
            {
                InterceptSynchronous(invocation);
            }
        }

        private bool IsAsyncMethod(MethodInfo method)
        {
            var hasAsyncAttribute = AttributeHelper.GetCustomAttribute<AsyncStateMachineAttribute>(method) != null;
            bool isAsync = (hasAsyncAttribute && typeof(Task).IsAssignableFrom(method.ReturnType));
            return isAsync;
        }

        protected virtual void InterceptSynchronous(IInvocation invocation)
        {
            invocation.Proceed();
        }

        protected virtual async Task InterceptAsync(Task task)
        {
            await task.ConfigureAwait(false);
        }

        protected virtual async Task<T> InterceptAsync<T>(Task<T> task)
        {
            T result = await task.ConfigureAwait(false);
            return result;
        }
    }
}
