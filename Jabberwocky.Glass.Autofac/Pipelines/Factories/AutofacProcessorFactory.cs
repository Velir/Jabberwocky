using System;
using Autofac;
using Jabberwocky.Glass.Autofac.Extensions;
using Jabberwocky.Glass.Autofac.Pipelines.Processors;
using Jabberwocky.Glass.Autofac.Util;
using Sitecore.Reflection;

namespace Jabberwocky.Glass.Autofac.Pipelines.Factories
{
	/// <summary>
	/// A Sitecore Factory adapter for Autofac 
	/// </summary>
	public class AutofacProcessorFactory : IFactory
	{
		protected static IContainer Container => AutofacConfig.ServiceLocator;

		public virtual object GetObject(string identifier)
		{
			var type = ResolveType(identifier);
			if (type == null) return null;

			// Includes Pipeline specific registrations that override existing defaults
			var scope = Container.BeginLifetimeScope(ConfigureRegistrationOverrides);
			try
			{
				var processor = scope.Resolve(type);

				// Assign LifetimeScope if possible
				var baseProcessor = processor as ProcessorLifetimeBase;
				if (baseProcessor != null)
				{
					baseProcessor.LifetimeScope = scope;
				}

				return processor;
			}
			catch
			{
				// If an error occurs during service resolution, but we have already created a lifetime scope, dispose it!
				scope.Dispose();
			}

			return null;
		}

		protected virtual void ConfigureRegistrationOverrides(ContainerBuilder builder)
		{
			builder.RegisterSitecorePipelineServices();
		}

		private static Type ResolveType(string identifier)
		{
			return identifier == null ? null : Type.GetType(identifier, false, true);
		}
	}
}
