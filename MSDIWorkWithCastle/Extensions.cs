using System;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace castlecoresample
{
    public static class Extensions
    {
        public static IServiceCollection AddDynamicProxyService<TService>(
            this IServiceCollection services,
            Func<IServiceProvider, TService> implementationFactory,
            params IInterceptor[] interceptors) where TService : class
        {
            services.TryAddSingleton<ProxyGenerator>();

            services.TryAddTransient<TService>(sp =>
            {
                var pg = sp.GetRequiredService<ProxyGenerator>();
                var implementation = implementationFactory(sp);
                var serv = pg.CreateInterfaceProxyWithTargetInterface<TService>(implementation, interceptors);
                return serv;
            });

            return services;
        }

    }
    
}