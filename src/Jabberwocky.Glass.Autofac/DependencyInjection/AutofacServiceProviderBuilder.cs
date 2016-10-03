using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Pipelines;

namespace Jabberwocky.Glass.Autofac.DependencyInjection
{
	public class AutofacServiceProviderBuilder : BaseServiceProviderBuilder
	{
		private const string PipelineName = "registerAutofacDependencies";

		protected override IServiceProvider BuildServiceProvider(IServiceCollection serviceCollection)
		{
			var args = new RegisterAutofacDependenciesPipelineArgs
			{
				ServiceCollection = serviceCollection,
				ContainerBuilder = new ContainerBuilder()
			};

			CorePipeline.Run(PipelineName, args, false);

			return new AutofacServiceProvider(args.BuiltContainer ?? args.ContainerBuilder.Build());
		}
	}
}
