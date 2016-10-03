using Autofac;
using Jabberwocky.Autofac.Modules;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;
using log4net;

namespace Jabberwocky.Autofac.Extras.Log4Net.Sc.Pipelines.RegisterAutofacDependencies
{
	public class RegisterLogInjectionModule : IProcessor<RegisterAutofacDependenciesPipelineArgs>
	{
		public void Process(RegisterAutofacDependenciesPipelineArgs pipelineArgs)
		{
			pipelineArgs.ContainerBuilder.RegisterModule(new LogInjectionModule<ILog>(LogManager.GetLogger));
		}
	}
}
