using System;
using System.Linq;
using Autofac.Core;
using Castle.DynamicProxy;
using Jabberwocky.Autofac.Modules.Aspected.Configuration;
using Jabberwocky.Autofac.Modules.Aspected.Strategies.Activation;
using Jabberwocky.Core.Testing;
using Jabberwocky.Core.Utils.Extensions;

namespace Jabberwocky.Autofac.Modules.Aspected.Strategies
{
	public class InterfaceProxyStrategy : ActivationReplacementStrategy
	{
		protected static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator(true);

		public override bool CanHandle(InterceptionContext context)
		{
			return context.ComponentServices.All(s => s.ServiceType.IsInterface);
		}

		protected override void CreateProxy(ActivatedInterceptionContext context)
		{
			var interceptors = context.Interceptors;
			var proxiedInterfaces = context.ProxyableInterfaces;
			var instance = context.ExistingInstance;
			var e = context.EventArgs;

			CreateInterfaceProxy(interceptors, e, proxiedInterfaces, instance);
		}

		protected static void CreateInterfaceProxy(IInterceptor[] interceptors, ActivatingEventArgs<object> e, Type[] proxiedInterfaces,
			object instance)
		{
			// If we don't have anything to proxy, then don't
			if (!proxiedInterfaces.Any())
			{
				return;
			}

			// intercept with all interfaces
			var primaryInterface = proxiedInterfaces.First();
			var secondaryInterfaces = proxiedInterfaces.Skip(1).ToArray();

			// Special handling for nested proxies...
			if (ProxyUtil.IsProxy(instance))
			{
				OverrideProxyInterceptors(interceptors, instance);
			}
			else
			{
				e.ReplaceInstance(ProxyGenerator.CreateInterfaceProxyWithTarget(primaryInterface, secondaryInterfaces, instance, interceptors));
			}
		}

		protected static void OverrideProxyInterceptors(IInterceptor[] interceptors, object instance)
		{
			dynamic proxy = DynamicWrapper.For(instance);
			var existingInterceptors = proxy.__interceptors as IInterceptor[];
			if (existingInterceptors != null)
			{
				interceptors = interceptors.Concat(existingInterceptors.Except(interceptors, (t1, t2) => t1.GetType() == t2.GetType())).ToArray();
				proxy.__interceptors = interceptors;
			}
		}
	}
}
