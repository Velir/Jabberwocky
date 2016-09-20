using Autofac;

namespace Jabberwocky.Glass.Autofac.DependencyInjection
{
	public abstract class LifetimeBase
	{
		internal ILifetimeScope LifetimeScope { get; set; }
	}
}
