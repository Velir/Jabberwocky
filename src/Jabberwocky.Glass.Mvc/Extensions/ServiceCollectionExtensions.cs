using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Jabberwocky.Core.Utils.Reflection;
using Jabberwocky.Glass.Mvc.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Jabberwocky.Glass.Mvc.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddMvcControllers(this IServiceCollection serviceCollection, params string[] assemblyNames)
		{
			assemblyNames = assemblyNames ?? new string[0];

			var assemblies = new[] { Assembly.GetExecutingAssembly() }.Concat(assemblyNames.Select(AssemblyManager.LoadAssemblySafe)).Distinct();

			var controllers = AssemblyManager.GetTypesImplementing<IController>(assemblies)
				.Where(controller => controller.Name.EndsWith("Controller", StringComparison.Ordinal));

			foreach (var controller in controllers)
			{
				serviceCollection.AddTransient(controller);
			}
		}

		public static void AddGlassViewModels(this IServiceCollection serviceCollection, params string[] assemblyNames)
		{
			assemblyNames = assemblyNames ?? new string[0];

			var assemblies = new[] { Assembly.GetExecutingAssembly() }.Concat(assemblyNames.Select(AssemblyManager.LoadAssemblySafe)).Distinct();

			var processors = AssemblyManager.GetTypesImplementing<InjectableGlassViewModelBase>(assemblies);

			foreach (var controller in processors)
			{
				serviceCollection.AddTransient(controller);
			}
		}
	}
}
