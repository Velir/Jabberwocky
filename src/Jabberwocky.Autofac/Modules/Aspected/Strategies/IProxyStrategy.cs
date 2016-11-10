using Jabberwocky.Autofac.Modules.Aspected.Configuration;

namespace Jabberwocky.Autofac.Modules.Aspected.Strategies
{
	public interface IProxyStrategy
	{
		bool CanHandle(InterceptionContext context);

		void CreateProxy(InterceptionContext context);
	}
}
