using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Pipelines.GetGlassLoaders;
using Jabberwocky.DependencyInjection.Scanning;

namespace Jabberwocky.Glass.Pipelines.GetGlassLoaders
{
	public class GetAttributeConfigurationLoaders : GetGlassLoadersProcessor
	{
		private static readonly WebHostAssemblyScanner AssemblyScanner = new WebHostAssemblyScanner();
		private readonly List<string> _assemblies = new List<string>();

		public override void Process(GetGlassLoadersPipelineArgs args)
		{
			var loader = new SitecoreAttributeConfigurationLoader(_assemblies.ToArray());
			args.Loaders.Add(loader);
		}

		protected void AddAssemblies(string assembly)
		{
			var assemblyNames = AssemblyScanner.FindMatchingAssemblyNames($"{assembly}.dll");
			_assemblies.AddRange(assemblyNames);
		}
	}
}
