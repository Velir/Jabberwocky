using System.Diagnostics;
using Jabberwocky.Extras.NewRelic.Sc.Reference;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;

namespace Jabberwocky.Extras.NewRelic.Sc.Pipelines.RenderRendering
{
	public class StartResponseTimeMetricProcessor : RenderRenderingProcessor
	{
		public override void Process(RenderRenderingArgs args)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			args.CustomData.Add(Constants.NewRelicBeginStartTimeKey, stopwatch);
		}
	}
}