using System;
using System.Linq;
using Jabberwocky.Glass.Factory.Builder.Loader;
using Jabberwocky.Glass.Factory.Caching;
using Jabberwocky.Glass.Factory.Configuration;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Util;

namespace Jabberwocky.Glass.Factory.Builder
{
	public class GlassFactoryBuilder : AbstractGlassFactoryBuilder
	{
		private readonly Func<ILookup<Type, GlassInterfaceMetadata>, IGlassTemplateCacheService> _templateCacheFactory;
		private readonly IGlassTypesLoader _typeLoader;
		private readonly IImplementationFactory _implFactory;

		public GlassFactoryBuilder(IConfigurationOptions options, 
			Func<ILookup<Type, GlassInterfaceMetadata>, IGlassTemplateCacheService> templateCacheFactory, 
			IGlassTypesLoader typeLoader, IImplementationFactory implFactory) 
			: base(options)
		{
			if (templateCacheFactory == null) throw new ArgumentNullException(nameof(templateCacheFactory));
			if (typeLoader == null) throw new ArgumentNullException(nameof(typeLoader));
			if (implFactory == null) throw new ArgumentNullException(nameof(implFactory));
			_templateCacheFactory = templateCacheFactory;
			_typeLoader = typeLoader;
			_implFactory = implFactory;
		}

		public override IGlassInterfaceFactory BuildFactory()
		{
			var implementedTypes = _typeLoader.LoadImplementations(Options.Assemblies);
			var templateCache = _templateCacheFactory(implementedTypes);

			return new GlassInterfaceFactory(templateCache, _implFactory);
		}
	}
}
