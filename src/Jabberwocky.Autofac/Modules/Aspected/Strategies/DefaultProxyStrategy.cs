using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Castle.DynamicProxy;
using Jabberwocky.Autofac.Modules.Aspected.Configuration;
using Jabberwocky.Core.Testing;

namespace Jabberwocky.Autofac.Modules.Aspected.Strategies
{
	public class DefaultProxyStrategy : IProxyStrategy
	{
		protected static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator(true);

		public bool CanHandle(InterceptionContext context)
		{
			return true;
		}

		public void CreateProxy(InterceptionContext context)
		{
			var componentServices = context.ComponentServices;
			var interceptors = context.Interceptors;
			var proxiedInterfaces = context.ProxyableInterfaces;
			var instance = context.ExistingInstance;
			var e = context.EventArgs;
			var type = context.InstanceType;

			if (componentServices.All(s => s.ServiceType.IsInterface))
			{
				CreateInterfaceProxy(interceptors, e, proxiedInterfaces, instance);
			}
			else
			{
				CreateClassProxy(interceptors, e, type, instance, proxiedInterfaces);
			}
		}

		protected static void CreateClassProxy(IInterceptor[] interceptors, ActivatingEventArgs<object> e, Type type, object instance,
			Type[] proxiedInterfaces)
		{
			// Ensure we don't try to proxy anything that can't be proxied
			if (type.IsSealed)
			{
				return;
			}

			var targetType = type;

			// Special handling for nested proxies...
			if (ProxyUtil.IsProxy(instance))
			{
				OverrideProxyInterceptors(interceptors, instance);
			}
			else
			{
				var proxyType = ProxyGenerator.ProxyBuilder.CreateClassProxyType(targetType, proxiedInterfaces,
					ProxyGenerationOptions.Default);

				var activator = new ReflectionActivator(proxyType,
					new DefaultConstructorFinder(),
					new MostParametersConstructorSelector(),
					GetConfiguredParams(ProxyGenerationOptions.Default, interceptors),
					Enumerable.Empty<Parameter>());

				e.ReplaceInstance(activator.ActivateInstance(e.Context, e.Parameters));

				// Dispose of the old instance, if necessary
				var oldInstance = instance as IDisposable;
				oldInstance?.Dispose();
			}
		}

		protected static void CreateInterfaceProxy(IInterceptor[] interceptors, ActivatingEventArgs<object> e, Type[] proxiedInterfaces,
			object instance)
		{
			// Ensure we don't proxy anything from Castle, to avoid proxying known-unknown proxies
			if (!proxiedInterfaces.Any())
			{
				return;
			}

			// intercept with all interfaces
			var theInterface = proxiedInterfaces.First();
			var interfaces = proxiedInterfaces.Skip(1).ToArray();

			// Special handling for nested proxies...
			if (ProxyUtil.IsProxy(instance))
			{
				OverrideProxyInterceptors(interceptors, instance);
			}
			else
			{
				e.ReplaceInstance(ProxyGenerator.CreateInterfaceProxyWithTarget(theInterface, interfaces, instance, interceptors));
			}
		}

		protected static void OverrideProxyInterceptors(IInterceptor[] interceptors, object instance)
		{
			dynamic proxy = DynamicWrapper.For(instance);
			var existingInterceptors = proxy.__interceptors as IInterceptor[];
			if (existingInterceptors != null)
			{
				interceptors = interceptors.Concat(existingInterceptors).ToArray();
				proxy.__interceptors = interceptors;
			}
		}

		protected static IEnumerable<Parameter> GetConfiguredParams(ProxyGenerationOptions options, IInterceptor[] interceptors)
		{
			List<Parameter> @params = new List<Parameter>();
			int position = 0;
			if (options.HasMixins)
			{
				@params.AddRange(options.MixinData.Mixins.Select(obj => (Parameter)new PositionalParameter(position++, obj)));
			}
			List<Parameter> list2 = @params;
			int position1 = position;
			int position2 = position1 + 1;
			PositionalParameter positionalParameter = new PositionalParameter(position1, interceptors);
			list2.Add(positionalParameter);
			if (options.Selector != null)
				@params.Add(new PositionalParameter(position2, options.Selector));

			return @params;
		}
	}
}
