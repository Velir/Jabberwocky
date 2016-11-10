using System;
using System.Linq;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Jabberwocky.Glass.Autofac.Extensions;
using Jabberwocky.Glass.Factory;
using Jabberwocky.Glass.Factory.Builder;
using Jabberwocky.Glass.Factory.Builder.Loader;
using Jabberwocky.Glass.Factory.Caching;
using Jabberwocky.Glass.Factory.Configuration;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Interceptors;
using Jabberwocky.Glass.Factory.Util;

namespace Jabberwocky.Glass.Autofac.Factory.Builder
{
	public class AutofacGlassFactoryBuilder : AbstractGlassFactoryBuilder
	{
		private readonly IContainer _container;
		private readonly Func<ILookup<Type, GlassInterfaceMetadata>, IGlassTemplateCacheService> _templateCacheFactory;
		private readonly IGlassTypesLoader _typeLoader;
		private readonly IImplementationFactory _implFactory;

		public AutofacGlassFactoryBuilder(IConfigurationOptions options, IContainer container,
			Func<ILookup<Type, GlassInterfaceMetadata>, IGlassTemplateCacheService> templateCacheFactory, 
			IGlassTypesLoader typeLoader, IImplementationFactory implFactory) 
			: base(options)
		{
			if (container == null) throw new ArgumentNullException(nameof(container));
			if (templateCacheFactory == null) throw new ArgumentNullException(nameof(templateCacheFactory));
			if (typeLoader == null) throw new ArgumentNullException(nameof(typeLoader));
			if (implFactory == null) throw new ArgumentNullException(nameof(implFactory));
			_container = container;
			_templateCacheFactory = templateCacheFactory;
			_typeLoader = typeLoader;
			_implFactory = implFactory;
		}

		public override IGlassInterfaceFactory BuildFactory()
		{
			var implementedTypes = _typeLoader.LoadImplementations(Options.Assemblies);
			var templateCache = _templateCacheFactory(implementedTypes);

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

			return new GlassInterfaceFactory(templateCache, _implFactory);
		}
	}
}
