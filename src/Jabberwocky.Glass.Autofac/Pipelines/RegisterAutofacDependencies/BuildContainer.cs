using Jabberwocky.Glass.Autofac.Extensions;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;

namespace Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies
{
	public class BuildContainer : IProcessor<RegisterAutofacDependenciesPipelineArgs>
	{
		public void Process(RegisterAutofacDependenciesPipelineArgs args)
		{
			// Seals the container, and registers itself
			args.BuiltContainer = args.ContainerBuilder.Build().RegisterContainer();
		}
	}
}