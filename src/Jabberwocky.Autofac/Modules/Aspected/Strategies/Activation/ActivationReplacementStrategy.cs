using Autofac.Core;
using Jabberwocky.Autofac.Modules.Aspected.Configuration;

namespace Jabberwocky.Autofac.Modules.Aspected.Strategies.Activation
{
	public abstract class ActivationReplacementStrategy : IProxyStrategy
	{
		public abstract bool CanHandle(InterceptionContext context);

		public virtual void CreateProxy(InterceptionContext context)
		{
			context.ComponentRegistration.Activating += (sender, e) =>
			{
				try
				{
					var activatedContext = CreateActivatedContext(context, e);

					CreateProxy(activatedContext);
				}
				catch
				{
					// do nothing
				}
			};
		}

		private static ActivatedInterceptionContext CreateActivatedContext(InterceptionContext context, ActivatingEventArgs<object> e)
		{
			var activatedContext = new ActivatedInterceptionContext
			{
				ComponentRegistry = context.ComponentRegistry,
				ComponentRegistration = context.ComponentRegistration,
				EventArgs = e,
				ExistingInstance = e.Instance,
				ComponentServices = context.ComponentServices,
				InstanceType = context.InstanceType,
				Interceptors = context.Interceptors,
				ProxyableInterfaces = context.ProxyableInterfaces
			};
			return activatedContext;
		}

		protected abstract void CreateProxy(ActivatedInterceptionContext context);
	}
}
