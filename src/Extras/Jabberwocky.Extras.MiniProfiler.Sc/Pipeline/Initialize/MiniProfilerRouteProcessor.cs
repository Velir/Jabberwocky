using Jabberwocky.Extras.MiniProfiler.Sc.Configuration;
using Sitecore.Pipelines;
using StackExchange.Profiling;

namespace Jabberwocky.Extras.MiniProfiler.Sc.Pipeline.Initialize
{
	public class MiniProfilerRouteProcessor
	{
		public virtual void Process(PipelineArgs args)
		{
			MiniProfilerHandler.Configure(new MiniProfilerOptions());
			MiniProfilerConfiguration.IsMiniProfilerInitialized = true;
		}
	}
}
