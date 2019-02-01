using Sitecore.Pipelines.HttpRequest;
using Profiler = StackExchange.Profiling.MiniProfiler;

namespace Jabberwocky.Extras.MiniProfiler.Sc.Pipeline.HttpRequestBegin
{
	public class StartMiniProfiler : HttpRequestProcessor
	{
		public override void Process(HttpRequestArgs args)
		{
			if (!Sitecore.Context.PageMode.IsExperienceEditor  && !args.RequestUrl.AbsolutePath.ToLowerInvariant().Contains("/sitecore/"))
			{
				Profiler.StartNew();
			}
		}
	}
}
