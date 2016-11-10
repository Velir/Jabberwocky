using System;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using Jabberwocky.Glass.Autofac.Pipelines.PipelineArgs;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;
using Jabberwocky.Glass.Autofac.Util;

namespace Jabberwocky.Glass.Autofac.Mvc.Pipelines.RegisterAutofacDependencies
{
	public class ConfigureMvcDependencyResolver : IProcessor<RegisterAutofacDependenciesPipelineArgs>
	{
		public void Process(RegisterAutofacDependenciesPipelineArgs pipelineArgs)
		{
			var container = AutofacConfig.ServiceLocator;
			if (container == null)
			{
				throw new InvalidOperationException($"{nameof(AutofacConfig)}.{nameof(AutofacConfig.ServiceLocator)} cannot be null. Ensure that the BuildContainer processor was called beforehand.");
			}

			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}
	}
}
