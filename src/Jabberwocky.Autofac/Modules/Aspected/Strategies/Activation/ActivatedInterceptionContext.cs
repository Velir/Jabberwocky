using Autofac.Core;
using Jabberwocky.Autofac.Modules.Aspected.Configuration;

namespace Jabberwocky.Autofac.Modules.Aspected.Strategies.Activation
{
	public class ActivatedInterceptionContext : InterceptionContext
	{
		public object ExistingInstance;
		public ActivatingEventArgs<object> EventArgs;
	}
}
