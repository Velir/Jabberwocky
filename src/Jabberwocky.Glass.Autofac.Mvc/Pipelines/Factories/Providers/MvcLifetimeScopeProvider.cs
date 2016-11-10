using Autofac;
using Autofac.Integration.Mvc;
using ILifetimeScopeProvider = Jabberwocky.Glass.Autofac.DependencyInjection.Factories.Providers.ILifetimeScopeProvider;

namespace Jabberwocky.Glass.Autofac.Mvc.Pipelines.Factories.Providers
{
    public class MvcLifetimeScopeProvider : DependencyInjection.Factories.Providers.ILifetimeScopeProvider
    {
        public ILifetimeScope GetLifetimeScope()
        {
            try
            {
                return AutofacDependencyResolver.Current?.RequestLifetimeScope;
            }
            catch
            {
                return null;
            }
        }
    }
}
