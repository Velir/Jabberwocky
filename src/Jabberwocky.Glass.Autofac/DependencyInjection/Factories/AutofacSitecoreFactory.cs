using System;
using Autofac;
using Jabberwocky.Glass.Autofac.Extensions;
using Jabberwocky.Glass.Autofac.Util;
using Sitecore.Reflection;

namespace Jabberwocky.Glass.Autofac.DependencyInjection.Factories
{
	/// <summary>
	/// A Sitecore Factory adapter for Autofac 
	/// </summary>
	[Obsolete("Use Sitecore DI instead")]
	public class AutofacSitecoreFactory : IFactory
	{
		protected static IContainer Container => AutofacConfig.ServiceLocator;

        public virtual object GetObject(string identifier)
		{
			var type = ResolveType(identifier);
			if (type == null) return null;

			// Includes Pipeline specific registrations that override existing defaults
            var scope = CreateLifetimeScope();
			try
			{
				var processor = scope.Resolve(type);

				// Assign LifetimeScope if possible
				var baseProcessor = processor as ILifetimeBase;
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

	    protected virtual ILifetimeScope CreateLifetimeScope()
	    {
            if (Container == null)
                throw new InvalidOperationException($"The '{nameof(Container)}' property was null. Ensure that the AutofacConfig.{nameof(AutofacConfig.ServiceLocator)} has been assigned.");

	        using (var scope = Container.BeginLifetimeScope())
	        {
	            var existingScopeProvider = scope.ResolveOptional<ILifetimeScopeFactory>();
	            var scopeResolver = existingScopeProvider?.GetCurrentLifetimeScope() ?? Container;
                
                return scopeResolver.BeginLifetimeScope(ConfigureRegistrationOverrides);
	        }
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
