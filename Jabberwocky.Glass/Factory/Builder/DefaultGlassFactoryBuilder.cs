using System;
using System.Linq;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Factory.Configuration;
using Jabberwocky.Glass.Factory.Implementation;

namespace Jabberwocky.Glass.Factory.Builder
{
	public class DefaultGlassFactoryBuilder : AbstractGlassFactoryBuilder
	{
		public DefaultGlassFactoryBuilder(IConfigurationOptions options) : base(options)
		{
		}

		public override IGlassInterfaceFactory BuildFactory(IImplementationFactory implFactory, Func<ISitecoreService> serviceFactory)
		{
			if (implFactory == null) throw new ArgumentNullException("implFactory");
			if (serviceFactory == null) throw new ArgumentNullException("serviceFactory");

			var assemblies = LoadAssemblies(Options).ToArray();
			var interfaceTypes = LoadInterfaces(assemblies).ToArray();
			var implementedTypes = LoadImplementations(assemblies, interfaceTypes);

			return new GlassInterfaceFactory(implementedTypes, implFactory, serviceFactory);
		}
	}
}
