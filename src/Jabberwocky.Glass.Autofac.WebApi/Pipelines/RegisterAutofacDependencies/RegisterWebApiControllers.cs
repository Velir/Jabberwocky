using Autofac.Extras.AttributeMetadata;
using Autofac.Integration.WebApi;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies.Base;

namespace Jabberwocky.Glass.Autofac.WebApi.Pipelines.RegisterAutofacDependencies
{
	public class RegisterWebApiControllers : BaseConfiguredAssemblyProcessor
	{
		public override void Process(RegisterAutofacDependenciesPipelineArgs args)
		{
			var assemblies = LoadAssemblies(GetConfiguredAssemblies(args));

			args.ContainerBuilder.RegisterApiControllers(assemblies).WithAttributeFilter();
		}
	}
}
