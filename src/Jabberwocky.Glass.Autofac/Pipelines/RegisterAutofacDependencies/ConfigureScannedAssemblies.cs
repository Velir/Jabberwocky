using System.Linq;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies.Base;

namespace Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies
{
	public class ConfigureScannedAssemblies : BaseConfiguredAssemblyProcessor
	{
		public override void Process(RegisterAutofacDependenciesPipelineArgs pipelineArgs)
		{
			pipelineArgs.ScanAssemblies = ConfiguredAssemblies.ToList();
		}
	}
}
