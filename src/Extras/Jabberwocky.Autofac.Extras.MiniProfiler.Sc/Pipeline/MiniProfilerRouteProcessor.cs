using Jabberwocky.Autofac.Extras.MiniProfiler.Util;
using Sitecore.Pipelines;
using StackExchange.Profiling;

namespace Jabberwocky.Autofac.Extras.MiniProfiler.Sc.Pipeline
{
	public class MiniProfilerRouteProcessor
	{
		public virtual void Process(PipelineArgs args)
		{
			MiniProfilerHandler.RegisterRoutes();
			MiniProfilerRuntime.MiniProfilerInitialized = true;
		}
	}
}
