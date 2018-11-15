using System;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;
using Jabberwocky.Core.Utils.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Jabberwocky.WebApi.Sc.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddWebApiControllers(this IServiceCollection serviceCollection, params string[] assemblyNames)
		{
			assemblyNames = assemblyNames ?? new string[0];

			var assemblies = new[] { Assembly.GetExecutingAssembly() }.Concat(assemblyNames.Select(AssemblyManager.LoadAssemblySafe)).Distinct();

			var controllers = AssemblyManager.GetTypesImplementing<IHttpController>(assemblies)
				.Where(controller => controller.Name.EndsWith("Controller", StringComparison.Ordinal));

			foreach (var controller in controllers)
			{
				serviceCollection.AddTransient(controller);
			}
		}
	}
}
