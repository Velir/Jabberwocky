using Autofac;
using Glass.Mapper.Sc;
using Jabberwocky.Core.Caching;
using Jabberwocky.Glass.Caching;
using Jabberwocky.Glass.Services;

namespace Jabberwocky.Glass.Autofac.Extensions
{
	public static class CacheRegistrationExtensions
	{
		public static void RegisterCacheServices(this ContainerBuilder builder)
		{
			// If necessary, register SiteContextService
			builder.RegisterType<SiteContextService>().As<ISiteContextService>().PreserveExistingDefaults();

			// Register Caches
			builder.RegisterType<SiteCache>().Named<ICacheProvider>("defaultCacheImplementor").WithMetadata("name", "default").SingleInstance();
			builder.RegisterType<SiteCache>().Named<ICacheProvider>("siteCacheImplementor").WithMetadata("name", "site").SingleInstance();
			builder.RegisterDecorator<ICacheProvider>((c, provider) => new SitecoreCacheDecorator(provider, GetSitecoreServiceForCache(c), c.Resolve<ISiteContextService>()), "siteCacheImplementor");
		}

		private static ISitecoreService GetSitecoreServiceForCache(IComponentContext c)
		{
			var sitecoreContext = c.Resolve<ISitecoreContext>();
			return sitecoreContext.Database == null
				? c.Resolve<ISitecoreService>()
				: sitecoreContext;
		}
	}
}
