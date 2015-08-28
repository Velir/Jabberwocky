using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using Glass.Mapper.Sc;
using Jabberwocky.Glass.Factory.Attributes;
using Jabberwocky.Glass.Factory.Configuration;
using Jabberwocky.Glass.Factory.Implementation;
using Jabberwocky.Glass.Factory.Util;

namespace Jabberwocky.Glass.Factory.Builder
{
	public abstract class AbstractGlassFactoryBuilder : IGlassFactoryBuilder
	{
		protected IConfigurationOptions Options { get; }

		protected AbstractGlassFactoryBuilder(IConfigurationOptions options)
		{
			if (options == null) throw new ArgumentNullException(nameof(options));
			Options = options;
		}

		public abstract IGlassInterfaceFactory BuildFactory(IImplementationFactory implFactory, Func<ISitecoreService> serviceFactory);

		// This needs to contain more information than just a list of Tuples... Should most likely be a dictionary/lookup that contains a complex object of templateId and interface type, and implementation type
		protected static ILookup<Type, GlassInterfaceMetadata> LoadImplementations(IEnumerable<Assembly> assemblies, IEnumerable<Type> interfaceTypes)
		{
			var abstractTypes = assemblies
				.SelectMany(assembly => assembly.GetTypes())
				.Where(type => type.IsAbstract)
				.Where(type => type.HasAttribute<GlassFactoryTypeAttribute>());

			var flattenedTypes = abstractTypes
				.SelectMany(abstractType => abstractType.GetInterfaces(),
					(abstractType, interfaceType) => new { Interface = interfaceType, Abstract = abstractType })
				.Where(tuple => interfaceTypes.Contains(tuple.Interface));

			return flattenedTypes.ToLookup(tuple => tuple.Interface, tuple => new GlassInterfaceMetadata
			{
				GlassType = GetGlassType(tuple.Abstract),
				ImplementationType = tuple.Abstract,
				IsFallback = GetIsFallback(tuple.Abstract)
			});
		}

		protected static bool GetIsFallback(Type abstractType)
		{
			var customAttribute = abstractType.GetCustomAttribute<GlassFactoryTypeAttribute>();	// overload: true?
			return customAttribute.IsFallback;
		}

		protected static Type GetGlassType(Type abstractType)
		{
			var customAttribute = abstractType.GetCustomAttribute<GlassFactoryTypeAttribute>();	// overload: true?
			return customAttribute.Type;
		}

		protected static IEnumerable<Type> LoadInterfaces(IEnumerable<Assembly> assemblies)
		{
			return assemblies
				.SelectMany(assembly => assembly.GetTypes())
				.Where(type => type.HasAttribute<GlassFactoryInterfaceAttribute>()); // Must be an interface, since AttributeUsage guarantees it
		}

		protected static IEnumerable<Assembly> LoadAssemblies(IConfigurationOptions options)
		{
			return options.Assemblies
				.Select(LoadAssembly)
				.Where(assembly => assembly != null);
		}

		private static Assembly LoadAssembly(string assemblyName)
		{
			try
			{
				return Assembly.Load(assemblyName);
			}
			catch
			{
				// Suppress assembly loading errors
			}
			return null;
		}
	}
}
