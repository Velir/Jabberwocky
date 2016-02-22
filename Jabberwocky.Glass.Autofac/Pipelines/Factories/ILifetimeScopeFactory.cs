using Autofac;

namespace Jabberwocky.Glass.Autofac.Pipelines.Factories
{
    public interface ILifetimeScopeFactory
    {
        ILifetimeScope GetCurrentLifetimeScope();
    }
}
