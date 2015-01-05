using Autofac;

namespace Jabberwocky.Glass.Autofac.Pipelines.Processors
{
	public abstract class ProcessorLifetimeBase
	{
		internal ILifetimeScope LifetimeScope { get; set; }
	}
}
