using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extras.AggregateService;
using Jabberwocky.Glass.Autofac.Attributes;

namespace Jabberwocky.Glass.Autofac.Extensions
{
	public static class AutowireRegistrationExtensions
	{
		public static void AutowireDependencies(this ContainerBuilder builder, bool preserveDefaults = false, params string[] assemblyNames)
		{
			var assemblies = new[] {Assembly.GetExecutingAssembly()}.Concat(assemblyNames.Select(Assembly.Load)).Distinct();

			foreach (var meta in assemblies.SelectMany(asm => asm.ExportedTypes)
				.Select(type => new { Type = type, Attr = type.GetCustomAttributes<AutowireServiceAttribute>(true).FirstOrDefault()})
				.Where(meta => meta.Attr != null))
			{
				if (meta.Type.IsInterface && meta.Attr.IsAggregateService)
				{
					builder.RegisterAggregateService(meta.Type);
					continue;
				}

				var registration = builder.RegisterType(meta.Type).AsImplementedInterfaces();
				registration = preserveDefaults ? registration.PreserveExistingDefaults() : registration;

				switch (meta.Attr.LifetimeScope)
				{
					case LifetimeScope.PerRequest:
						registration.InstancePerRequest();
                        break;
					case LifetimeScope.PerScope:
						registration.InstancePerLifetimeScope();
                        break;
					case LifetimeScope.SingleInstance:
						registration.SingleInstance();
                        break;
					case LifetimeScope.NoTracking:
						registration.ExternallyOwned();
						break;
					case LifetimeScope.Default:
					default:
						registration.InstancePerDependency();
						break;
				}
			}
		}
	}
}
