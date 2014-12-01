using System.Linq;
using System.Reflection;
using Autofac;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;

namespace Jabberwocky.Glass.Autofac.Extensions
{
	public static class SitecorePipelineRegistrationExtensions
	{

		/// <summary>
		/// Registers any custom Sitecore Pipeline Processors that implement the IProcessor interface 
		/// </summary>
		/// <param name="builder">The container builder.</param>
		/// <param name="assemblyNames">Assemblies to scan for IProcessor implementors.</param>
		/// <returns>
		/// Container Builder
		/// </returns>
		public static ContainerBuilder RegisterProcessors(this ContainerBuilder builder, params string[] assemblyNames)
		{
			return builder.RegisterProcessors(assemblyNames.Select(Assembly.Load).ToArray());
		}

		/// <summary>
		/// Registers any custom Sitecore Pipeline Processors that implement the IProcessor interface 
		/// </summary>
		/// <param name="builder">The container builder.</param>
		/// <param name="assemblies">Assemblies to scan for IProcessor implementors.</param>
		/// <returns>
		/// Container Builder
		/// </returns>
		public static ContainerBuilder RegisterProcessors(this ContainerBuilder builder, params Assembly[] assemblies)
		{
			builder.RegisterAssemblyTypes(assemblies).AsClosedTypesOf(typeof(IProcessor<>));

			return builder;
		}

	}
}
