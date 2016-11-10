using Autofac.Extras.AttributeMetadata;
using Autofac.Integration.Mvc;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies.Base;

namespace Jabberwocky.Glass.Autofac.Mvc.Pipelines.RegisterAutofacDependencies
{
	public class RegisterMvcControllers : BaseConfiguredAssemblyProcessor
	{
		public override void Process(RegisterAutofacDependenciesPipelineArgs args)
		{
			var assemblies = LoadAssemblies(GetConfiguredAssemblies(args));

			args.ContainerBuilder.RegisterControllers(assemblies).WithAttributeFilter();
		}
	}
}
