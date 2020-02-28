using Castle.DynamicProxy;

namespace AutofacAsyncInterceptor
{
    public class AsyncInterceptorAdaper<TAsyncInterceptor> : AsyncDeterminationInterceptor
      where TAsyncInterceptor : IAsyncInterceptor
    {
        public AsyncInterceptorAdaper(TAsyncInterceptor asyncInterceptor)
            : base(asyncInterceptor)
        { }
    }
}
