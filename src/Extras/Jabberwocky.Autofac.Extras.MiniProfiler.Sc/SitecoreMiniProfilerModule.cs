using Jabberwocky.Autofac.Extras.MiniProfiler.Configuration;
using Jabberwocky.Autofac.Modules.Aspected.Strategies;

namespace Jabberwocky.Autofac.Extras.MiniProfiler.Sc
{
	public class SitecoreMiniProfilerModule : MiniProfilerModule
	{
		public SitecoreMiniProfilerModule(params string[] assemblies)
			: this(new MiniProfilerConfiguration(assemblies, assemblies))
		{
		}

		public SitecoreMiniProfilerModule(IProxyStrategy[] strategies, params string[] assemblies)
			: this(new MiniProfilerConfiguration(assemblies, assemblies, strategies: strategies))
		{
		}

		public SitecoreMiniProfilerModule(MiniProfilerConfiguration config)
			: base(OverwriteConfig(config))
		{
		}

		private static MiniProfilerConfiguration OverwriteConfig(MiniProfilerConfiguration config)
		{
			config.IsMiniProfilerInitialized = false;

			return config;
		}
	}
}
