﻿using System;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Factory.Builder.Loader;
using Jabberwocky.Glass.Factory.Caching;
using Jabberwocky.Glass.Factory.Configuration;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Interceptors;

namespace Jabberwocky.Glass.Factory.Builder
{
	public class DefaultGlassFactoryBuilder : AbstractGlassFactoryBuilder
	{
		private readonly Func<ISitecoreService> _serviceFactory;

		public DefaultGlassFactoryBuilder(IConfigurationOptions options, Func<ISitecoreService> serviceFactory) : base(options)
		{
			if (serviceFactory == null) throw new ArgumentNullException(nameof(serviceFactory));
			_serviceFactory = serviceFactory;
		}

		public override IGlassAdapterFactory BuildFactory()
		{
			IImplementationFactory implFactory = null;

			var implementedTypes = new DefaultGlassTypeLoader().LoadImplementations(Options.Assemblies);
			var templateCache = new GlassTemplateCacheService(implementedTypes, _serviceFactory);
			implFactory = new ProxyImplementationFactory((t, model) => new FallbackInterceptor(t, model, templateCache, implFactory));

			return new GlassAdapterFactory(templateCache, implFactory);
		}
	}
}
