using Autofac;

namespace Jabberwocky.Glass.Autofac.DependencyInjection.Factories.Providers
{
    internal interface ILifetimeScopeProvider
    {
        ILifetimeScope GetLifetimeScope();
    }
}
