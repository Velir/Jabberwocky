using System;
using System.Linq;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web;
using Jabberwocky.Core.Caching;
using Jabberwocky.DependencyInjection.Sc.Configuration;
using Jabberwocky.Glass.Caching;
using Jabberwocky.Glass.Extensions;
using Jabberwocky.Glass.Factory;
using Jabberwocky.Glass.Factory.Builder;
using Jabberwocky.Glass.Factory.Builder.Loader;
using Jabberwocky.Glass.Factory.Caching;
using Jabberwocky.Glass.Factory.Configuration;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Interceptors;
using Jabberwocky.Glass.Factory.Util;
using Jabberwocky.Glass.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Data;
using Sitecore.DependencyInjection;

namespace Jabberwocky.Glass.DependencyInjection
{
    public class Configurator : AbstractServicesConfigurator
    {
        public override void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ISiteContextService, SiteContextService>();

            serviceCollection.AddSingleton<SiteCache>();
            serviceCollection.AddScoped<ICacheProvider>(sp => new SitecoreCacheDecorator(sp.GetService<SiteCache>(), GetSitecoreServiceForCache(sp), sp.GetService<ISiteContextService>()));

            serviceCollection.AddScoped<IItemService, ItemService>();
            serviceCollection.AddScoped<ILinkService, LinkService>();

            serviceCollection.AddSingleton<Func<Database, ISitecoreService>>(_ => CreateSitecoreService);
            serviceCollection.AddSingleton<Func<ISitecoreService>>(_ => Get<ISitecoreService>);
            serviceCollection.AddScoped(_ => CreateSitecoreContextService());
            serviceCollection.AddScoped(_ => CreateRequestContext());
            serviceCollection.AddScoped(_ => CreateLegacySitecoreContextService());

            serviceCollection.AddProcessors(AssemblyNames);

            serviceCollection.AddTransient<FallbackInterceptor>();
            serviceCollection.AddTransient<IGlassTypesLoader, DefaultGlassTypeLoader>();
            serviceCollection.AddTransient<IImplementationFactory, ProxyImplementationFactory>();

            serviceCollection.AddTransient<IGlassFactoryBuilder, DefaultGlassFactoryBuilder>(c => new DefaultGlassFactoryBuilder(
                new ConfigurationOptions(false, AssemblyNames),
                c.GetService<Func<ISitecoreService>>(),
                c));
            serviceCollection.AddSingleton<IGlassInterfaceFactory>(c => c.GetService<IGlassFactoryBuilder>().BuildFactory());
            serviceCollection.AddTransient<Func<ILookup<Type, GlassInterfaceMetadata>, IGlassTemplateCacheService>>(c => lookup => new GlassTemplateCacheService(lookup, c.GetService<Func<ISitecoreService>>()));
            serviceCollection.AddSingleton<IGlassTemplateCacheService>(c => ((GlassInterfaceFactory)c.GetService<IGlassInterfaceFactory>()).TemplateCacheService);
        }

        private static ISitecoreService GetSitecoreServiceForCache(IServiceProvider sp)
        {
            var sitecoreContext = sp.GetService<ISitecoreContext>();
            return sitecoreContext.Database == null
                ? sp.GetService<ISitecoreService>()
                : sitecoreContext;
        }

        private static ISitecoreService CreateSitecoreService(Database database)
        {
            return new SitecoreService(database);
        }

        private static ISitecoreService CreateSitecoreContextService()
        {
            var sitecoreServiceThunk = Get<Func<Database, ISitecoreService>>();
            return sitecoreServiceThunk(Sitecore.Context.Database ?? Sitecore.Configuration.Factory.GetDatabase("web"));
        }

        private static ISitecoreContext CreateLegacySitecoreContextService()
        {
            return new SitecoreContext(Sitecore.Context.Database ?? Sitecore.Configuration.Factory.GetDatabase("web"));
        }

        private static T Get<T>()
        {
            return ServiceLocator.ServiceProvider.GetService<T>();
        }

        private static IRequestContext CreateRequestContext()
        {
            return new RequestContext(Get<ISitecoreService>());
        }
    }
}
