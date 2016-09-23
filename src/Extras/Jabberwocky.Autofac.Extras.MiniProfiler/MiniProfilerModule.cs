using Jabberwocky.Autofac.Extras.MiniProfiler.Configuration;
using Jabberwocky.Autofac.Extras.MiniProfiler.Util;
using Jabberwocky.Autofac.Modules.Aspected;
using Jabberwocky.Autofac.Modules.Aspected.Strategies;

namespace Jabberwocky.Autofac.Extras.MiniProfiler
{
	/// <summary>
	///     An autofac module for wiring up interceptors for deep profiling via MiniProfiler
	/// </summary>
	/// <remarks>
	///     Inspired by:
	///     http://stackoverflow.com/questions/22782086/autofac-global-interface-interceptor-with-autofac-extras-dynamicproxy2
	/// </remarks>
	public class MiniProfilerModule : AspectInterceptionModule
	{
		public MiniProfilerModule(params string[] assemblies)
			: this(new MiniProfilerConfiguration(assemblies, assemblies))
		{
		}

		public MiniProfilerModule(IProxyStrategy[] strategies, params string[] assemblies)
			: this(new MiniProfilerConfiguration(assemblies, assemblies, strategies: strategies))
		{
		}

		public MiniProfilerModule(MiniProfilerConfiguration config)
			: base(config)
		{
			MiniProfilerRuntime.MiniProfilerInitialized = config.IsMiniProfilerInitialized;
		}
	}
}
