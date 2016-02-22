using Autofac;

namespace Jabberwocky.Glass.Autofac.Pipelines.Factories.Providers
{
    internal interface ILifetimeScopeProvider
    {
        ILifetimeScope GetLifetimeScope();
    }
}
