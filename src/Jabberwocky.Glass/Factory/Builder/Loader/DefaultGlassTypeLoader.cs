using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Internal;
using Jabberwocky.Glass.Factory.Attributes;
using Jabberwocky.Glass.Factory.Util;

namespace Jabberwocky.Glass.Factory.Builder.Loader
{
	public class DefaultGlassTypeLoader : IGlassTypesLoader
	{
		public ILookup<Type, GlassInterfaceMetadata> LoadImplementations(IEnumerable<string> assemblyNames)
		{
			var assemblies = LoadAssemblies(assemblyNames).ToArray();
			var interfaceTypes = LoadInterfaces(assemblies).ToArray();

			var abstractTypes = assemblies
				.SelectMany(assembly => assembly.GetTypes())
				.Where(type => type.IsAbstract)
				.Where(type => type.GetAttribute<GlassFactoryTypeAttribute>() != null);

			var flattenedTypes = abstractTypes
				.SelectMany(abstractType => abstractType.GetInterfaces(),
					(abstractType, interfaceType) => new { Interface = interfaceType, Abstract = abstractType })
				.Where(tuple => interfaceTypes.Contains(tuple.Interface));

			return flattenedTypes.ToLookup(tuple => tuple.Interface, tuple => new GlassInterfaceMetadata
			{
				GlassType = GetGlassType(tuple.Abstract),
				ImplementationType = tuple.Abstract,
				IsFallback = GetIsFallback(tuple.Abstract),
				ZIndex = GetZIndex(tuple.Abstract)
			});
		}

		internal IEnumerable<Type> LoadInterfaces(IEnumerable<Assembly> assemblies)
		{
			return assemblies
				.SelectMany(assembly => assembly.GetTypes())
				.Where(type => type.GetAttribute<GlassFactoryInterfaceAttribute>() != null); // Must be an interface, since AttributeUsage guarantees it
		}
		internal IEnumerable<Assembly> LoadAssemblies(IEnumerable<string> assemblyNames)
		{
			return assemblyNames
				.Select(LoadAssembly)
				.Where(assembly => assembly != null);
		}

		protected static bool GetIsFallback(Type abstractType)
		{
			var customAttribute = abstractType.GetCustomAttribute<GlassFactoryTypeAttribute>(); // overload: true?
			return customAttribute.IsFallback;
		}

		protected static Type GetGlassType(Type abstractType)
		{
			var customAttribute = abstractType.GetCustomAttribute<GlassFactoryTypeAttribute>(); // overload: true?
			return customAttribute.Type;
		}

		protected static int GetZIndex(Type abstractType)
		{
			var customAttribute = abstractType.GetCustomAttribute<GlassFactoryTypeAttribute>(); // overload: true?
			return customAttribute.ZIndex;
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
