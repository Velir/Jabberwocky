using Jabberwocky.DependencyInjection.Autowire.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Jabberwocky.DependencyInjection.Sc.Configuration
{
	public class AutowireServiceConfigurator : AbstractServicesConfigurator
	{
		public override void Configure(IServiceCollection serviceCollection)
		{
			// Auto-wire with assemblies from current web host
			serviceCollection.AutowireDependencies(AssemblyNames);
		}
	}
}
