using Jabberwocky.Autofac.Extensions;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies.Base;

namespace Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies
{
	public class AutowireDependencies : BaseConfiguredAssemblyProcessor
	{
		public override void Process(RegisterAutofacDependenciesPipelineArgs args)
		{
			var assemblies = GetConfiguredAssemblies(args);

			// Auto-Wire
			args.ContainerBuilder.AutowireDependencies(assemblyNames: assemblies);
		}
	}
}
