using Jabberwocky.Extras.MiniProfiler.Sc.Configuration;
using Sitecore.Pipelines;
using StackExchange.Profiling;
using Profiler = StackExchange.Profiling.MiniProfiler;

namespace Jabberwocky.Extras.MiniProfiler.Sc.Pipeline.Initialize
{
	public class MiniProfilerRouteProcessor
	{
		public virtual void Process(PipelineArgs args)
		{
			Profiler.Configure(new MiniProfilerOptions());
			MiniProfilerConfiguration.IsMiniProfilerInitialized = true;
		}
	}
}
