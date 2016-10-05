using Jabberwocky.Glass.Autofac.DependencyInjection.Providers;
using Jabberwocky.Glass.Autofac.Extensions;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;
using Sitecore.DependencyInjection;

namespace Jabberwocky.Glass.Autofac.Pipelines.RegisterAutofacDependencies
{
	public class BuildContainer : IProcessor<RegisterAutofacDependenciesPipelineArgs>
	{
		public void Process(RegisterAutofacDependenciesPipelineArgs args)
		{
			// Update the existing container
			var delegatingProvider = ServiceLocator.ServiceProvider as AutofacDelegatingServiceProvider;
			if (delegatingProvider != null)
			{
				args.ContainerBuilder.Update(delegatingProvider.RootContainer);

				// Seals the container, and registers itself
				delegatingProvider.RootContainer.RegisterContainer();

				// Update the args with the built container
				args.BuiltContainer = delegatingProvider.RootContainer;
			}
		}
	}
}