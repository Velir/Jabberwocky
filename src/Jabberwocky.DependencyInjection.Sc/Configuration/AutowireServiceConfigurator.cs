using Jabberwocky.DependencyInjection.Autowire.Extensions;
using Jabberwocky.DependencyInjection.Scanning;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Jabberwocky.DependencyInjection.Sc.Configuration
{
	public class AutowireServiceConfigurator : IServicesConfigurator
	{
		private static readonly WebHostAssemblyScanner AssemblyScanner = new WebHostAssemblyScanner();

		public void Configure(IServiceCollection serviceCollection)
		{
			// Auto-wire with assemblies from current web host
			serviceCollection.AutowireDependencies(AssemblyScanner.FindMatchingAssemblyNames("*.dll"));
		}
	}
}
