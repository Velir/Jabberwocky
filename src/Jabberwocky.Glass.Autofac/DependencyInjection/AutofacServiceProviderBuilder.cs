using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Jabberwocky.Glass.Autofac.DependencyInjection.Providers;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Jabberwocky.Glass.Autofac.DependencyInjection
{
	public class AutofacServiceProviderBuilder : DefaultBaseServiceProviderBuilder
	{
		protected override IServiceProvider BuildServiceProvider(IServiceCollection serviceCollection)
		{
			// Build a default container with only the conforming service collection registrations
			var builder = new ContainerBuilder();
			builder.Populate(serviceCollection);

			var container = builder.Build();

			return new AutofacDelegatingServiceProvider(new AutofacServiceProvider(container), serviceCollection)
			{
				RootContainer = container
			};
		}
	}
}
