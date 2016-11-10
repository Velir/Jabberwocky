using Autofac.Extensions.DependencyInjection;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;

namespace Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies
{
	public class PopulateServiceCollection : IProcessor<RegisterAutofacDependenciesPipelineArgs>
	{
		public void Process(RegisterAutofacDependenciesPipelineArgs pipelineArgs)
		{
			pipelineArgs.ContainerBuilder.Populate(pipelineArgs.ServiceCollection);
		}
	}
}
