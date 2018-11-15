using System.Linq;
using System.Reflection;
using Jabberwocky.Core.Utils.Reflection;
using Jabberwocky.Glass.Pipelines.Processors;
using Microsoft.Extensions.DependencyInjection;

namespace Jabberwocky.Glass.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddProcessors(this IServiceCollection serviceCollection, params string[] assemblyNames)
		{
			assemblyNames = assemblyNames ?? new string[0];

			var assemblies = new[] { Assembly.GetExecutingAssembly() }.Concat(assemblyNames.Select(AssemblyManager.LoadAssemblySafe)).Distinct();

			var processors = AssemblyManager.GetTypesImplementing(typeof(IProcessor<>), assemblies);

			foreach (var controller in processors)
			{
				serviceCollection.AddTransient(controller);
			}
		}
	}
}
