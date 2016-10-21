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
		public bool IncludeSitecoreRegistrations { get; set; } = true;

		public HashSet<string> IncludeNamespaces { get; set; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		public HashSet<string> ExcludeNamespaces { get; set; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		public HashSet<string> ExcludeTypes { get; set; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		public HashSet<string> ExcludeAssemblies { get; set; } = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

		public override void Process(RegisterAutofacDependenciesPipelineArgs args)
		{
			var sitecoreServiceAssemblies = args.ServiceCollection.Select(description => description.ServiceType.Assembly?.GetName()?.Name);
			var instrumentedSitecoreAssemblies = sitecoreServiceAssemblies.Where(n => !string.IsNullOrEmpty(n)).Distinct().ToArray();
			var includedSitecoreNamespaces = instrumentedSitecoreAssemblies.Concat(new[] { "Sitecore" });

			MiniProfilerConfiguration config = CreateConfig(args);
			config.Assemblies = IncludeSitecoreRegistrations
				? config.Assemblies.Concat(instrumentedSitecoreAssemblies).Distinct()
				: config.Assemblies;
			config.IncludeNamespaces = IncludeSitecoreRegistrations
				? config.IncludeNamespaces.Concat(includedSitecoreNamespaces).Distinct()
				: config.IncludeNamespaces;
			
			args.ContainerBuilder.RegisterModule(new SitecoreMiniProfilerModule(config));
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

		public MiniProfilerConfiguration CreateConfig(RegisterAutofacDependenciesPipelineArgs args)
		{
			var assemblies = GetConfiguredAssemblies(args);

			// Explicitly declare the includedNamespaces to be the set of assemblies IFF no overrides are provided
			var includedNamespaces = IncludeNamespaces.Any()
				? IncludeNamespaces.ToArray()
				: assemblies;

			return new MiniProfilerConfiguration(assemblies)
			{
				IncludeNamespaces = includedNamespaces,
				ExcludeNamespaces = ExcludeNamespaces,
				ExcludeTypes = ExcludeTypes,
				ExcludeAssemblies = ExcludeAssemblies
			};
		}
	}
}
