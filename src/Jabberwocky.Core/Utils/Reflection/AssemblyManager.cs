using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jabberwocky.Core.Utils.Reflection
{
	public static class AssemblyManager
	{
		public static Assembly LoadAssemblySafe(string assemblyName)
		{
			try
			{
				return Assembly.Load(assemblyName);
			}
			catch
			{
				return null;
			}
		}

		public static bool TryLoadAssembly(string assemblyName, out Assembly assembly)
		{
			assembly = LoadAssemblySafe(assemblyName);

			return assembly != null;
		}

		public static IEnumerable<Type> TryGetExportedTypes(Assembly asm)
		{
			try
			{
				return asm.ExportedTypes;
			}
			catch
			{
				// Unable to load types
				return Enumerable.Empty<Type>();
			}
		}

		public static Type[] GetTypesImplementing<T>(IEnumerable<Assembly> assemblies)
		{
			return GetTypesImplementing(typeof(T), assemblies);
		}

		public static Type[] GetTypesImplementing(Type targetType, IEnumerable<Assembly> assemblies)
		{
			if (assemblies == null)
			{
				return new Type[0];
			}

			return assemblies
				.Where(assembly => !assembly.IsDynamic)
				.SelectMany(TryGetExportedTypes)
				.Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition && targetType.IsAssignableFrom(type))
				.ToArray();
		}
	}
}
