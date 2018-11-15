using System.Collections.Generic;
using System.Xml;
using Jabberwocky.DependencyInjection.Scanning;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Configuration;
using Sitecore.DependencyInjection;

namespace Jabberwocky.DependencyInjection.Sc.Configuration
{
	public abstract class AbstractServicesConfigurator : IServicesConfigurator
	{
		protected static readonly WebHostAssemblyScanner AssemblyScanner = new WebHostAssemblyScanner();
		protected string[] AssemblyNames { get; private set; }

		protected AbstractServicesConfigurator()
		{
			XmlDocument configuration = ConfigReader.GetConfiguration();
			var node = configuration.SelectSingleNode($"/sitecore/services/Jabberwocky.ScannedAssemblies");

			var assemblies = new List<string>();
			foreach (XmlNode childNode in node.ChildNodes)
			{
				assemblies.AddRange(AssemblyScanner.FindMatchingAssemblyNames($"{childNode.InnerText}.dll"));
			}

			AssemblyNames = assemblies.ToArray();
		}

		public abstract void Configure(IServiceCollection serviceCollection);
	}
}
