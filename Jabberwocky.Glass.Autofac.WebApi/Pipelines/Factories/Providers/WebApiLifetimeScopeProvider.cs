using System.Web.Http;
using System.Web.Http.Dependencies;
using Autofac;
using Autofac.Integration.WebApi;
using Jabberwocky.Glass.Autofac.Pipelines.Factories.Providers;

namespace Jabberwocky.Glass.Autofac.WebApi.Pipelines.Factories.Providers
{
    public class WebApiLifetimeScopeProvider : ILifetimeScopeProvider
    {
        public ILifetimeScope GetLifetimeScope()
        {
            // If using the Autofac.WebApi integration package, this dependency should be of type 'AutofacWebApiDependencyResolver'
            var resolver = GlobalConfiguration.Configuration.DependencyResolver as IDependencyScope;

            return resolver?.GetRequestLifetimeScope();
        }
    }
}
