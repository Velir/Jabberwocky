using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using Jabberwocky.Autofac.Modules.Aspected.Strategies;

namespace Jabberwocky.Autofac.Modules.Aspected.Configuration
{
	public class AspectConfiguration
	{
		public IEnumerable<IInterceptor> Interceptors { get; set; }
		public IEnumerable<IProxyStrategy> Strategies { get; set; }
		public IEnumerable<string> Assemblies { get; set; }
		public IEnumerable<string> IncludeNamespaces { get; set; }
		public IEnumerable<string> ExcludeNamespaces { get; set; }
		public IEnumerable<string> ExcludeTypes { get; set; }
		public IEnumerable<string> ExcludeAssemblies { get; set; }

		public bool AutowireAspect { get; set; } = true;

		public AspectConfiguration(IInterceptor[] interceptors, IEnumerable<string> assemblies, IEnumerable<IProxyStrategy> strategies = null, IEnumerable<string> includeNamespaces = null, IEnumerable<string> excludeNamespaces = null, IEnumerable<string> excludeTypes = null, IEnumerable<string> excludeAssemblies = null)
		{
			Interceptors = interceptors;
			Strategies = strategies ?? Enumerable.Empty<IProxyStrategy>();
			Assemblies = assemblies ?? Enumerable.Empty<string>();
			IncludeNamespaces = includeNamespaces ?? Enumerable.Empty<string>();
			ExcludeNamespaces = excludeNamespaces ?? Enumerable.Empty<string>();
			ExcludeTypes = excludeTypes ?? Enumerable.Empty<string>();
			ExcludeAssemblies = excludeAssemblies ?? Enumerable.Empty<string>();
		}
	}
}
