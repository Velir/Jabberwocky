using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jabberwocky.Core.Utils.Reflection;
using Jabberwocky.DependencyInjection.Scanning;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;

namespace Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies.Base
{
	public abstract class BaseConfiguredAssemblyProcessor : IProcessor<RegisterAutofacDependenciesPipelineArgs>
	{
		// WebHostAssemblyScanner is required to remain thread-safe when used within a processor like this
		protected static readonly WebHostAssemblyScanner AssemblyScanner = new WebHostAssemblyScanner();

		protected HashSet<string> ConfiguredAssemblies = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		
		public bool IncludeScanAssemblies { get; set; } = true;

		public abstract void Process(RegisterAutofacDependenciesPipelineArgs args);

		public virtual void AddAssembly(string assemblyName)
		{
			string[] matchingAssemblies = assemblyName.Contains("*") ? AssemblyScanner.FindMatchingAssemblyNames($"{assemblyName}.dll") : new [] { assemblyName };
			foreach (string assembly in matchingAssemblies)
			{
				ConfiguredAssemblies.Add(assembly);
			}
		}

		protected virtual string[] GetConfiguredAssemblies(RegisterAutofacDependenciesPipelineArgs args)
		{
			return (IncludeScanAssemblies
				? args.ScanAssemblies.Concat(ConfiguredAssemblies)
				: ConfiguredAssemblies)
					.Distinct(StringComparer.InvariantCultureIgnoreCase)
					.ToArray();
		}

		protected virtual Assembly[] LoadAssemblies(IEnumerable<string> assemblyNames)
		{
			return assemblyNames.Select(TryLoad).Where(a => a != null).ToArray();
		}

		protected Assembly TryLoad(string assemblyName)
		{
			return AssemblyManager.LoadAssemblySafe(assemblyName);
		}
	}
}
