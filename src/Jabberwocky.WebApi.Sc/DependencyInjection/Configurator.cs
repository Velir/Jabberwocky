using Jabberwocky.DependencyInjection.Sc.Configuration;
using Jabberwocky.WebApi.Sc.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Jabberwocky.WebApi.Sc.DependencyInjection
{
	public class Configurator : AbstractServicesConfigurator
	{
		public override void Configure(IServiceCollection serviceCollection)
		{
			serviceCollection.AddWebApiControllers(AssemblyNames);
		}
	}
}
