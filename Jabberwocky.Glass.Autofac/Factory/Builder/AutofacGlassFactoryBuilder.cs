using System;
using System.Linq;
using Autofac;
using Autofac.Extras.DynamicProxy2;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Autofac.Extensions;
using Jabberwocky.Glass.Factory;
using Jabberwocky.Glass.Factory.Builder;
using Jabberwocky.Glass.Factory.Configuration;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Interceptors;

namespace Jabberwocky.Glass.Autofac.Factory.Builder
{
	public class AutofacGlassFactoryBuilder : AbstractGlassFactoryBuilder
	{
		private readonly IContainer _container;

		public AutofacGlassFactoryBuilder(IConfigurationOptions options, IContainer container) : base(options)
		{
			if (container == null) throw new ArgumentNullException("container");
			_container = container;
		}

		public override IGlassInterfaceFactory BuildFactory(IImplementationFactory implFactory, Func<ISitecoreService> serviceFactory)
		{
			var assemblies = LoadAssemblies(Options).ToArray();
			var interfaceTypes = LoadInterfaces(assemblies).ToArray();
			var implementedTypes = LoadImplementations(assemblies, interfaceTypes);

			// Build out registrations
			var builder = new ContainerBuilder();
			foreach (var type in implementedTypes.SelectMany(_ => _).Select(metadata => metadata.ImplementationType).Distinct())
			{
				builder.RegisterType(type).AsSelf()
					.EnableFallbackClassInterceptors()
					.InterceptedBy(typeof(FallbackInterceptor))
					.ExternallyOwned();
			}

			// Update existing container with new registrations
			builder.Update(_container);

			return new GlassInterfaceFactory(implementedTypes, implFactory, serviceFactory);
		}
	}
}
