using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;

namespace Jabberwocky.Glass.Autofac.Extensions
{
	public static class SitecorePipelineRegistrationExtensions
	{
		/// <summary>
		///  This is the master database, that should only be used 'master' context
		/// </summary>
		private const string MasterDatabaseName = "master";

		private const string JabberwockyMvcDll = "Jabberwocky.Glass.Autofac.Mvc";

		/// <summary>
		/// Registers any custom Sitecore Pipeline Processors that implement the IProcessor interface 
		/// </summary>
		/// <param name="builder">The container builder.</param>
		/// <param name="assemblyNames">Assemblies to scan for IProcessor implementors.</param>
		/// <returns>
		/// Container Builder
		/// </returns>
		public static ContainerBuilder RegisterProcessors(this ContainerBuilder builder, string[] assemblyNames)
		{
			return builder.RegisterProcessors(assemblyNames.Select(TryLoadAssembly).ToArray());
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
			var asm = new[] {TryLoadAssembly(JabberwockyMvcDll)}.Concat(assemblies).Where(a => a != null).Distinct().ToArray();

			builder.RegisterAssemblyTypes(asm).AsClosedTypesOf(typeof(IProcessor<>));
			
			return builder;
		}

		[Obsolete("Need to figure out how to do this appropriately, so we can vary registrations by pipeline")]
		internal static ContainerBuilder RegisterSitecorePipelineServices(this ContainerBuilder builder)
		{
			// Register custom ISitecoreService behavior for custom lifetime scopes
			//builder.Register(c => new SitecoreService(MasterDatabaseName)).As<ISitecoreService>();

			return builder;
		}

		private static Assembly TryLoadAssembly(string assemblyName)
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
	}
}
