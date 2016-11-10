using System.Collections.Generic;
using Castle.DynamicProxy;
using Jabberwocky.Autofac.Extras.MiniProfiler.Interceptors;
using Jabberwocky.Autofac.Modules.Aspected.Configuration;
using Jabberwocky.Autofac.Modules.Aspected.Strategies;

namespace Jabberwocky.Autofac.Extras.MiniProfiler.Configuration
{
	public sealed class MiniProfilerConfiguration : AspectConfiguration
	{
		internal bool IsMiniProfilerInitialized;

		public MiniProfilerConfiguration(
			IEnumerable<string> assemblies,
			IEnumerable<string> includeNamespaces = null,
			IEnumerable<string> excludeNamespaces = null,
			IEnumerable<string> excludeTypes = null,
			IEnumerable<string> excludeAssemblies = null,
			IProxyStrategy[] strategies = null)
			: base(
				new IInterceptor[] {new AsyncProfilingInterceptor()}, assemblies, strategies,
				includeNamespaces, excludeNamespaces, excludeTypes, excludeAssemblies)
		{
		}
	}
}
