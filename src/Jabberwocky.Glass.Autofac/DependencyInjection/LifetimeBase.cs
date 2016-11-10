using Autofac;

namespace Jabberwocky.Glass.Autofac.DependencyInjection
{
	public abstract class LifetimeBase : ILifetimeBase
	{
	   ILifetimeScope ILifetimeBase.LifetimeScope { get; set; }
	}

    internal interface ILifetimeBase
    {
        ILifetimeScope LifetimeScope { get; set; }
    }
}
