using System.Linq;
using Castle.DynamicProxy;
using Jabberwocky.Autofac.Modules.Aspected.Configuration;
using Jabberwocky.Autofac.Modules.Aspected.Strategies.Activation;
using Jabberwocky.Core.Utils.Reflection;
using Jabberwocky.Glass.Factory.Attributes;

namespace Jabberwocky.Glass.Autofac.Aspects.Strategies
{
	public class GlassAdapterFactoryStrategy : ActivationReplacementStrategy
	{
		private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator(true);

		public override bool CanHandle(InterceptionContext context)
		{
			// Only operate on Glass Interface Factory class proxies
			return context.ComponentServices.All(service => service.ServiceType.GetCustomAttributeSafe<GlassFactoryTypeAttribute>() != null);
		}

		protected override void CreateProxy(ActivatedInterceptionContext context)
		{
			// Create an interface-based proxy to wrap the underlying class-based proxy
			var proxiedInterfaces = context.ProxyableInterfaces;
			if (!proxiedInterfaces.Any()) return;

			// intercept with all interfaces
			var primaryInterface = proxiedInterfaces.First();
			var otherInterfaces = proxiedInterfaces.Skip(1).ToArray();

			context.EventArgs.ReplaceInstance(ProxyGenerator.CreateInterfaceProxyWithTarget(primaryInterface, otherInterfaces, context.ExistingInstance, context.Interceptors));
		}
	}
}
