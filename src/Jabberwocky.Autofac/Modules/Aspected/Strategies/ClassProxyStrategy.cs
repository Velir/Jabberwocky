using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Autofac.Core.Registration;
using Castle.DynamicProxy;
using Jabberwocky.Autofac.Modules.Aspected.Configuration;
using Jabberwocky.Core.Testing;

namespace Jabberwocky.Autofac.Modules.Aspected.Strategies
{
	public class ClassProxyStrategy : IProxyStrategy
	{
		protected static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator(true);

		public bool CanHandle(InterceptionContext context)
		{
			return true;
		}

		public void CreateProxy(InterceptionContext context)
		{
			var interceptors = context.Interceptors;
			var proxiedInterfaces = context.ProxyableInterfaces;
			var type = context.InstanceType;

			CreateClassProxy(context, type, proxiedInterfaces, interceptors);
		}

		protected static void CreateClassProxy(InterceptionContext context, Type type, Type[] proxiedInterfaces,
			IInterceptor[] interceptors)
		{
			var registration = context.ComponentRegistration as ComponentRegistration;
			var reflectionActivator = registration?.Activator as ReflectionActivator;
			if (reflectionActivator == null)
				return;

			var proxyType = ProxyGenerator.ProxyBuilder.CreateClassProxyType(type, proxiedInterfaces,
				ProxyGenerationOptions.Default);
			
			dynamic activator = DynamicWrapper.For(reflectionActivator);
			var configuredParameters = activator._defaultParameters as IEnumerable<Parameter> ?? Enumerable.Empty<Parameter>();
			var configuredProperties = activator._configuredProperties as IEnumerable<Parameter> ?? Enumerable.Empty<Parameter>();

			registration.Activator = new ReflectionActivator(proxyType,
				reflectionActivator.ConstructorFinder,
				reflectionActivator.ConstructorSelector,
				GetConfiguredParams(ProxyGenerationOptions.Default, interceptors).Concat(configuredParameters),
				configuredProperties);
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
