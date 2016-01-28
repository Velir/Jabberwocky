using Autofac;
using Autofac.Integration.Mvc;
using ILifetimeScopeProvider = Jabberwocky.Glass.Autofac.Pipelines.Factories.Providers.ILifetimeScopeProvider;

namespace Jabberwocky.Glass.Autofac.Mvc.Pipelines.Factories.Providers
{
    public class MvcLifetimeScopeProvider : ILifetimeScopeProvider
    {
        public ILifetimeScope GetLifetimeScope()
        {
            return AutofacDependencyResolver.Current?.RequestLifetimeScope;
        }
    }
}
