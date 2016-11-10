using Jabberwocky.Glass.Autofac.Mvc.Extensions;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies.Base;

namespace Jabberwocky.Glass.Autofac.Mvc.Pipelines.RegisterAutofacDependencies
{
	public class RegisterGlassMvcServices : BaseConfiguredAssemblyProcessor
	{
		public override void Process(RegisterAutofacDependenciesPipelineArgs args)
		{
			args.ContainerBuilder.RegisterGlassMvcServices(GetConfiguredAssemblies(args));
		}
	}
}
