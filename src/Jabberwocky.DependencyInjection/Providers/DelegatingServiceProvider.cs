using System;
using Microsoft.Extensions.DependencyInjection;

namespace Jabberwocky.DependencyInjection.Providers
{
	public class DelegatingServiceProvider : IServiceProvider
	{
		public IServiceProvider ServiceProvider { get; set; }
		public IServiceCollection ServiceCollection { get; set; }

		public DelegatingServiceProvider(IServiceProvider serviceProvider, IServiceCollection serviceCollection)
		{
			if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
			if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));
			ServiceProvider = serviceProvider;
			ServiceCollection = serviceCollection;
		}

		public object GetService(Type serviceType)
		{
			return ServiceProvider.GetService(serviceType);
		}
	}
}