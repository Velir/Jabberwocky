using System;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.ModelCache;
using Glass.Mapper.Sc.Web.Mvc;
using Jabberwocky.DependencyInjection.Extensions;
using Jabberwocky.DependencyInjection.Sc.Configuration;
using Jabberwocky.Glass.Mvc.Extensions;
using Jabberwocky.Glass.Mvc.Models.Factory;
using Jabberwocky.Glass.Mvc.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Jabberwocky.Glass.Mvc.DependencyInjection
{
	public class Configurator : AbstractServicesConfigurator
	{
		public override void Configure(IServiceCollection serviceCollection)
		{
			serviceCollection.AddMvcControllers(AssemblyNames);

            serviceCollection.AddScopedWithFuncFactory(_ => CreateGlassHtml());
            serviceCollection.AddScopedWithFuncFactory(_ => CreateMvcContext());
            serviceCollection.AddScoped<IRenderingContextService, RenderingContextService>();
			serviceCollection.AddScoped<IViewModelFactory, ViewModelFactory>();
			serviceCollection.AddSingleton<IModelCacheManager, ModelCacheManager>();

            serviceCollection.AddSingleton<Func<IRenderingContextService>>(_ => Get<IRenderingContextService>);
            serviceCollection.AddSingleton<Func<IViewModelFactory>>(_ => Get<IViewModelFactory>);

			serviceCollection.AddGlassViewModels(AssemblyNames);
		}

        private static IGlassHtml CreateGlassHtml()
        {
            return new GlassHtml(Get<ISitecoreService>());
        }

        private static IMvcContext CreateMvcContext()
        {
            return new MvcContext(Get<ISitecoreService>(), Get<IGlassHtml>());
        }

		private static T Get<T>()
        {
            return ServiceLocator.ServiceProvider.GetService<T>();
        }
	}
}
