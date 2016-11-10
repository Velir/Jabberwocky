using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs
{
	public class RegisterAutofacDependenciesPipelineArgs : Sitecore.Pipelines.PipelineArgs
	{
		public IServiceCollection ServiceCollection { get; set; }

		public ContainerBuilder ContainerBuilder { get; set; }

		public IContainer BuiltContainer { get; set; }

		public List<string> ScanAssemblies { get; set; }

		public RegisterAutofacDependenciesPipelineArgs()
		{
			ScanAssemblies = new List<string>();
		}
	}
}
