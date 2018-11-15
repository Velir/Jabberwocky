using Jabberwocky.DependencyInjection.AggregateService.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Jabberwocky.DependencyInjection.Sc.Configuration
{
	public class AggregateServiceConfigurator : AbstractServicesConfigurator
	{
		public override void Configure(IServiceCollection serviceCollection)
		{
			// Auto-wire with assemblies from current web host
			serviceCollection.RegisterAggregateServices(AssemblyNames);
		}
	}
}
