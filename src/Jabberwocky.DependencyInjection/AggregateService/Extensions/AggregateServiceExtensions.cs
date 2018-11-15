using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Jabberwocky.Core.Utils.Reflection;
using Jabberwocky.DependencyInjection.AggregateService.Attributes;
using Jabberwocky.DependencyInjection.AggregateService.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace Jabberwocky.DependencyInjection.AggregateService.Extensions
{
	public static class AggregateServiceRegistrationExtensions
	{
		private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

		public static void RegisterAggregateServices(this IServiceCollection collection, params string[] assemblyNames)
		{
			assemblyNames = assemblyNames ?? new string[0];

			var assemblies = new[] { Assembly.GetExecutingAssembly() }.Concat(assemblyNames.Select(AssemblyManager.LoadAssemblySafe)).Distinct();

			foreach (var meta in assemblies.SelectMany(AssemblyManager.TryGetExportedTypes)
				.Select(type => new { Type = type, Attr = type.GetCustomAttributesSafe<AggregateServiceAttribute>(true).FirstOrDefault() })
				.Where(meta => meta.Attr != null))
			{
				collection.AddTransient(meta.Type, c => ProxyGenerator.CreateInterfaceProxyWithoutTarget(meta.Type, new ResolvingInterceptor(meta.Type, c)));
			}
		}
	}
}
