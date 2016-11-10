using System;
using System.Web.Http;
using Autofac.Integration.WebApi;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;
using Jabberwocky.Glass.Autofac.Util;

namespace Jabberwocky.Glass.Autofac.WebApi.Pipelines.RegisterAutofacDependencies
{
	public class ConfigureWebApiDependencyResolver : IProcessor<RegisterAutofacDependenciesPipelineArgs>

	{
		public void Process(RegisterAutofacDependenciesPipelineArgs pipelineArgs)
		{
			var container = AutofacConfig.ServiceLocator;
			if (container == null)
			{
				throw new InvalidOperationException($"{nameof(AutofacConfig)}.{nameof(AutofacConfig.ServiceLocator)} cannot be null. Ensure that the BuildContainer processor was called beforehand.");
			}

			GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
		}
	}
}
