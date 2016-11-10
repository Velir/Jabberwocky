using Autofac;

namespace Jabberwocky.Glass.Autofac.DependencyInjection.Factories
{
    public interface ILifetimeScopeFactory
    {
        ILifetimeScope GetCurrentLifetimeScope();
    }
}
