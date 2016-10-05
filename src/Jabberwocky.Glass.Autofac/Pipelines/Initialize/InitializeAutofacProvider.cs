using Autofac;
using Autofac.Extensions.DependencyInjection;
using Jabberwocky.DependencyInjection.Providers;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace Jabberwocky.Glass.Autofac.Pipelines.Initialize
{
	public class InitializeAutofacProvider : IProcessor<Sitecore.Pipelines.PipelineArgs>
	{
		private const string PipelineName = "registerAutofacDependencies";

		public void Process(Sitecore.Pipelines.PipelineArgs pipelineArgs)
		{
			var delegatingProvider = ServiceLocator.ServiceProvider as DelegatingServiceProvider;

			if (delegatingProvider == null)
			{
				Log.Warn($"Unable to configure Autofac Service Provider. Expected type of DelegatingServiceProvider, but got '{ServiceLocator.ServiceProvider?.GetType()}' instead.", this);
				return;
			}

			var args = new RegisterAutofacDependenciesPipelineArgs
			{
				ServiceCollection = delegatingProvider.ServiceCollection,
				ContainerBuilder = new ContainerBuilder()
			};

			CorePipeline.Run(PipelineName, args, false);

			// Update the delegated provider to use Autofac instead of Sitecore's default
			delegatingProvider.ServiceProvider = new AutofacServiceProvider(args.BuiltContainer ?? args.ContainerBuilder.Build());
		}
	}
}
