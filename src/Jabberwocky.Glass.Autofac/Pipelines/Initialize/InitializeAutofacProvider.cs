using Autofac;
using Jabberwocky.Glass.Autofac.DependencyInjection.Providers;
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
			var delegatingProvider = ServiceLocator.ServiceProvider as AutofacDelegatingServiceProvider;

			if (delegatingProvider == null)
			{
				Log.Warn($"Unable to configure Autofac Service Provider. Expected type of AutofacDelegatingServiceProvider, but got '{ServiceLocator.ServiceProvider?.GetType()}' instead.", this);
				return;
			}

			// Update the existing container with configured registrations
			var args = new RegisterAutofacDependenciesPipelineArgs
			{
				ServiceCollection = delegatingProvider.ServiceCollection,
				ContainerBuilder = new ContainerBuilder()
			};

			CorePipeline.Run(PipelineName, args, false);
		}
	}
}
