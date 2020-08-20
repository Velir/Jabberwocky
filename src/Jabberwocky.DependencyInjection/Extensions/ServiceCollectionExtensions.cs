using System;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace Jabberwocky.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScopedWithFuncFactory<TService, TImplementation>(
          this IServiceCollection services)
          where TService : class
          where TImplementation : class, TService
        {
            return services.AddWithFuncFactory<TService, TImplementation>(ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddScopedWithFuncFactory<TService>(
          this IServiceCollection services,
          Func<IServiceProvider, TService> factory)
          where TService : class
        {
            return services.AddWithFuncFactory(factory, ServiceLifetime.Scoped);
        }

        public static IServiceCollection AddTransientWithFuncFactory<TService, TImplementation>(
          this IServiceCollection services)
          where TService : class
          where TImplementation : class, TService
        {
            return services.AddWithFuncFactory<TService, TImplementation>(ServiceLifetime.Transient);
        }

        public static IServiceCollection AddTransientWithFuncFactory<TService>(
          this IServiceCollection services,
          Func<IServiceProvider, TService> factory)
          where TService : class
        {
            return services.AddWithFuncFactory(factory, ServiceLifetime.Transient);
        }

        private static IServiceCollection AddWithFuncFactory<TService, TImplementation>(
          this IServiceCollection services,
          ServiceLifetime lifetime)
          where TService : class
          where TImplementation : class, TService
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (!Enum.IsDefined(typeof(ServiceLifetime), lifetime))
                throw new InvalidEnumArgumentException(nameof(lifetime), (int)lifetime, typeof(ServiceLifetime));

            services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
            return services.AddFuncFactory<TService>();
        }

        private static IServiceCollection AddWithFuncFactory<TService>(
          this IServiceCollection services,
          Func<IServiceProvider, TService> factory,
          ServiceLifetime lifetime)
          where TService : class
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (!Enum.IsDefined(typeof(ServiceLifetime), lifetime))
                throw new InvalidEnumArgumentException(nameof(lifetime), (int)lifetime, typeof(ServiceLifetime));

            services.Add(new ServiceDescriptor(typeof(TService), factory, lifetime));
            return services.AddFuncFactory<TService>();
        }

        private static IServiceCollection AddFuncFactory<TService>(this IServiceCollection services)
          where TService : class
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddSingleton<Func<TService>>(sp => () => sp.GetService(typeof(TService)) as TService);
            return services;
        }
    }
}
