using System;
using Autofac.Core;
using Castle.DynamicProxy;

namespace Jabberwocky.Autofac.Modules.Aspected.Configuration
{
	public class InterceptionContext
	{
		public IInterceptor[] Interceptors;
		public ActivatingEventArgs<object> EventArgs;
		public IServiceWithType[] ComponentServices;
		public Type[] ProxyableInterfaces;
		public object ExistingInstance;
		public Type InstanceType;
	}
}
