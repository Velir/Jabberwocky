using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extras.AggregateService;
using Jabberwocky.Autofac.Attributes;
using Jabberwocky.Core.Utils.Reflection;
using Jabberwocky.DependencyInjection.Autowire.Attributes;

namespace Jabberwocky.Autofac.Extensions
{
	public static class AutowireRegistrationExtensions
	{
		public static void AutowireDependencies(this ContainerBuilder builder, bool preserveDefaults = false, params string[] assemblyNames)
		{
			assemblyNames = assemblyNames ?? new string[0];

			var assemblies = new[] {Assembly.GetExecutingAssembly()}.Concat(assemblyNames.Select(AssemblyManager.LoadAssemblySafe)).Distinct();

			foreach (var meta in assemblies.SelectMany(asm => asm.ExportedTypes)
				.Select(GetRegistrationMetadata)
				.Where(meta => meta.Attr != null))
			{
				if (meta.IsAggregateService)
				{
					builder.RegisterAggregateService(meta.Type);
					continue;
				}

				var registration = builder.RegisterType(meta.Type).AsImplementedInterfaces();
				registration = preserveDefaults ? registration.PreserveExistingDefaults() : registration;

				switch (meta.Attr.LifetimeScope)
				{
					case LifetimeScope.PerScope:
						registration.InstancePerLifetimeScope();
                        break;
					case LifetimeScope.SingleInstance:
						registration.SingleInstance();
                        break;
					case LifetimeScope.Default:
					default:
						registration.InstancePerDependency();
						break;
				}

				if (meta.Attr.RegisterAsSelf)
				{
					registration.AsSelf();
				}

				if (meta.IsExternallyOwned)
				{
					registration.ExternallyOwned();
				}
			}
		}

		private static RegistrationMetadata GetRegistrationMetadata(Type type)
		{
			return new RegistrationMetadata
			{
				Type = type,
				Attr = type.GetCustomAttributesSafe<AutowireServiceAttribute>().FirstOrDefault(),
				IsExternallyOwned = type.GetCustomAttributesSafe<ExternallyOwnedAttribute>().Any(),
				IsAggregateService = type.GetCustomAttributesSafe<AggregateServiceAttribute>().Any()
			};
		}

		private struct RegistrationMetadata
		{
			public Type Type;
			public AutowireServiceAttribute Attr;
			public bool IsExternallyOwned;
			public bool IsAggregateService;
		}
	}
}
