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
            ServiceLifetime serviceLifetime,
            params IInterceptor[] interceptors) where TService : class
        {
            services.TryAddSingleton<ProxyGenerator>();

            Func<IServiceProvider, TService> proxyFactory = sp =>
            {
                var pg = sp.GetRequiredService<ProxyGenerator>();
                var implementation = implementationFactory(sp);
                var serv = pg.CreateInterfaceProxyWithTargetInterface<TService>(implementation, interceptors);
                return serv;
            };

            ServiceDescriptor serviceDescriptor = new ServiceDescriptor(typeof(TService), proxyFactory, serviceLifetime);
            services.TryAdd(serviceDescriptor);
            return services;
        }
    }
}