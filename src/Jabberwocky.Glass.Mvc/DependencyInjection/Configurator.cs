using Glass.Mapper.Sc;
using Glass.Mapper.Sc.ModelCache;
using Glass.Mapper.Sc.Web.Mvc;
using Jabberwocky.DependencyInjection.Sc.Configuration;
using Jabberwocky.Glass.Mvc.Extensions;
using Jabberwocky.Glass.Mvc.Models.Factory;
using Jabberwocky.Glass.Mvc.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Jabberwocky.Glass.Mvc.DependencyInjection
{
	public class Configurator : AbstractServicesConfigurator
	{
		public override void Configure(IServiceCollection serviceCollection)
		{
			serviceCollection.AddMvcControllers(AssemblyNames);

			serviceCollection.AddTransient<IGlassHtml, GlassHtml>();
			serviceCollection.AddTransient<IMvcContext, MvcContext>();
			serviceCollection.AddTransient<IRenderingContextService, RenderingContextService>();
			serviceCollection.AddTransient<IViewModelFactory, ViewModelFactory>();
			serviceCollection.AddSingleton<IModelCacheManager, ModelCacheManager>();
			
			serviceCollection.AddGlassViewModels(AssemblyNames);
		}
	}
}
