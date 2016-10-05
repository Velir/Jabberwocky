using System;
using Autofac;
using Jabberwocky.DependencyInjection.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Jabberwocky.Glass.Autofac.DependencyInjection.Providers
{
	public class AutofacDelegatingServiceProvider : DelegatingServiceProvider
	{
		public IContainer RootContainer { get; set; }

		public AutofacDelegatingServiceProvider(IServiceProvider provider, IServiceCollection serviceCollection) 
			: base(provider, serviceCollection)
		{
		}
	}
}
