using Jabberwocky.Glass.Autofac.Extensions;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies.Base;

namespace Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies
{
	public class RegisterProcessors : BaseConfiguredAssemblyProcessor
	{
		public override void Process(RegisterAutofacDependenciesPipelineArgs args)
		{
			args.ContainerBuilder.RegisterProcessors(GetConfiguredAssemblies(args));
		}
	}
}
