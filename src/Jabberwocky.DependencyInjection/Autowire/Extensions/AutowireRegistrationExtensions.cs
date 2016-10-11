using System.Linq;
using System.Reflection;
using Jabberwocky.Core.Utils.Reflection;
using Jabberwocky.DependencyInjection.Autowire.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Jabberwocky.DependencyInjection.Autowire.Extensions
{
	public static class AutowireRegistrationExtensions
	{
		public static void AutowireDependencies(this IServiceCollection collection, params string[] assemblyNames)
		{
			assemblyNames = assemblyNames ?? new string[0];

			var assemblies = new[] { Assembly.GetExecutingAssembly() }.Concat(assemblyNames.Select(AssemblyManager.LoadAssemblySafe)).Distinct();

			foreach (var meta in assemblies.SelectMany(asm => asm.ExportedTypes)
				.Select(type => new { Type = type, Attr = type.GetCustomAttributesSafe<AutowireServiceAttribute>(true).FirstOrDefault() })
				.Where(meta => meta.Attr != null))
			{
				// Configure the lifetime scope
				ServiceLifetime lifetime;
				switch (meta.Attr.LifetimeScope)
				{
					case LifetimeScope.PerScope:
						lifetime = ServiceLifetime.Scoped;
						break;
					case LifetimeScope.SingleInstance:
						lifetime = ServiceLifetime.Singleton;
						break;
					case LifetimeScope.Default:
					default:
						lifetime = ServiceLifetime.Transient;
						break;
				}

				// Register as implemented interfaces
				var implementedInterfaces = meta.Type.GetInterfaces();
				foreach (var implementedInterface in implementedInterfaces)
				{
					collection.Add(new ServiceDescriptor(implementedInterface, meta.Type, lifetime));
				}

				// Register as itself as well
				if (meta.Attr.RegisterAsSelf)
				{
					collection.Add(new ServiceDescriptor(meta.Type, meta.Type, lifetime));
				}
			}
		}
	}
}
