using Sitecore.Pipelines.HttpRequest;
using Profiler = StackExchange.Profiling.MiniProfiler;

namespace Jabberwocky.Extras.MiniProfiler.Sc.Pipeline.HttpRequestEnd
{
	public class StopMiniProfiler : HttpRequestProcessor
	{
		public override void Process(HttpRequestArgs args)
		{
#if DEBUG
			Profiler.Current?.Stop();
#endif
		}
	}
}
