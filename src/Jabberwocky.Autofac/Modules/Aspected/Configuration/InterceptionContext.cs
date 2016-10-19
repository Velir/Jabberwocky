using System;
using Autofac.Core;
using Castle.DynamicProxy;

namespace Jabberwocky.Autofac.Modules.Aspected.Configuration
{
	public class InterceptionContext
	{
		public IComponentRegistry ComponentRegistry;
		public IComponentRegistration ComponentRegistration;
		
		public IInterceptor[] Interceptors;
		public IServiceWithType[] ComponentServices;
		public Type[] ProxyableInterfaces;
		public Type InstanceType;
	}
}
