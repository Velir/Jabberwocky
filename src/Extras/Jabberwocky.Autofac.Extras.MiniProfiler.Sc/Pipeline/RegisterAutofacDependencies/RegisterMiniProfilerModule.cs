using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Jabberwocky.Autofac.Extras.MiniProfiler.Configuration;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies.Base;

namespace Jabberwocky.Autofac.Extras.MiniProfiler.Sc.Pipeline.RegisterAutofacDependencies
{
	public class RegisterMiniProfilerModule : BaseConfiguredAssemblyProcessor
	{
		public MiniProfilerConfigurationWrapper Configuration { get; set; } = new MiniProfilerConfigurationWrapper();

		public override void Process(RegisterAutofacDependenciesPipelineArgs args)
		{
			var sitecoreServiceAssemblies = args.ServiceCollection.Select(description => description.ServiceType.Assembly?.GetName()?.Name);
			var instrumentedSitecoreAssemblies = sitecoreServiceAssemblies.Where(n => !string.IsNullOrEmpty(n)).Distinct().ToArray();
			var includedSitecoreNamespaces = instrumentedSitecoreAssemblies.Concat(new[] { "Sitecore" });

			MiniProfilerConfiguration config = Configuration;
			config.Assemblies = Configuration.IncludeSitecoreRegistrations
				? config.Assemblies.Concat(instrumentedSitecoreAssemblies)
				: config.Assemblies;
			config.IncludeNamespaces = Configuration.IncludeSitecoreRegistrations
				? config.IncludeNamespaces.Concat(includedSitecoreNamespaces)
				: config.IncludeNamespaces;
			
			args.ContainerBuilder.RegisterModule(new SitecoreMiniProfilerModule(config));
		}
	}

	public class MiniProfilerConfigurationWrapper
	{
		public bool IncludeSitecoreRegistrations { get; set; } = true;

		public HashSet<string> Assemblies { get; set; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		public HashSet<string> IncludeNamespaces { get; set; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		public HashSet<string> ExcludeNamespaces { get; set; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		public HashSet<string> ExcludeTypes { get; set; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		public HashSet<string> ExcludeAssemblies { get; set; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

		public void AddAssembly(string assembly)
		{
			Assemblies.Add(assembly);
		}

		public void IncludeNamespace(string @namespace)
		{
			IncludeNamespaces.Add(@namespace);
		}

		public void ExcludeNamespace(string @namespace)
		{
			ExcludeAssemblies.Add(@namespace);
		}
		public void ExcludeType(string type)
		{
			ExcludeTypes.Add(type);
		}

		public void ExcludeAssembly(string assembly)
		{
			ExcludeAssemblies.Add(assembly);
		}

		public static implicit operator MiniProfilerConfiguration(MiniProfilerConfigurationWrapper wrapper)
		{
			// Explicitly declare the includedNamespaces to be the set of assemblies IFF no overrides are provided
			var includedNamespaces = wrapper.IncludeNamespaces.Any()
				? wrapper.IncludeNamespaces
				: wrapper.Assemblies;

			return new MiniProfilerConfiguration(wrapper.Assemblies.ToArray())
			{
				IncludeNamespaces = includedNamespaces,
				ExcludeNamespaces = wrapper.ExcludeNamespaces,
				ExcludeTypes = wrapper.ExcludeTypes,
				ExcludeAssemblies = wrapper.ExcludeAssemblies
			};
		} 
	}
}
