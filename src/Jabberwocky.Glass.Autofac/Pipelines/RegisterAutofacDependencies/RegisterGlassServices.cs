using Jabberwocky.Glass.Autofac.Extensions;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;

namespace Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies
{
	public class RegisterGlassServices : IProcessor<RegisterAutofacDependenciesPipelineArgs>
	{
		public void Process(RegisterAutofacDependenciesPipelineArgs pipelineArgs)
		{
			pipelineArgs.ContainerBuilder.RegisterGlassServices();
		}
	}
}
