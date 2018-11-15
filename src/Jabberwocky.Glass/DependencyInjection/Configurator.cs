using System;
using System.Linq;
using Glass.Mapper.Sc;
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

namespace Jabberwocky.Glass.DependencyInjection
{
	public class Configurator : AbstractServicesConfigurator
	{
		/// <summary>
		/// This is the default database that should be used to target the ISitecoreContext and ISitecoreService implementations
		/// </summary>
		private const string DefaultDatabaseName = "web";

		/// <summary>
		/// This is the 'core' database
		/// </summary>
		private const string CoreDatabaseName = "core";

		public override void Configure(IServiceCollection serviceCollection)
		{
			serviceCollection.AddTransient<ISiteContextService, SiteContextService>();

			serviceCollection.AddSingleton<ICacheProvider, SiteCache>();
			serviceCollection.Decorate<ICacheProvider>((provider, sp) => new SitecoreCacheDecorator(provider, GetSitecoreServiceForCache(sp), sp.GetService<ISiteContextService>()));

			serviceCollection.AddTransient<IItemService, ItemService>();
			serviceCollection.AddTransient<ILinkService, LinkService>();
			serviceCollection.AddTransient<ISitecoreContext, SitecoreContext>();

			serviceCollection.AddTransient<ISitecoreService>(sp => {
				var context = sp.GetService<ISitecoreContext>();
				return context?.Database != null && context.Database.Name != CoreDatabaseName
					? (ISitecoreService)context
					: new SitecoreService(DefaultDatabaseName);
			});
			serviceCollection.AddTransient<Func<ISitecoreService>>(c => c.GetService<ISitecoreService>);
			
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
	}
}
