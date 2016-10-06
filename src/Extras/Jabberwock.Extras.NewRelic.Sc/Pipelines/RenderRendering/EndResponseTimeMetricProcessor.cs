using System.Diagnostics;
using Jabberwocky.Extras.NewRelic.Sc.Reference;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using NR = NewRelic;

namespace Jabberwocky.Extras.NewRelic.Sc.Pipelines.RenderRendering
{
	public class EndResponseTimeMetricProcessor : RenderRenderingProcessor
	{
		public override void Process(RenderRenderingArgs args)
		{
			if (!args.CustomData.ContainsKey(Constants.NewRelicBeginStartTimeKey)) return;

			var stopwatch = args.CustomData[Constants.NewRelicBeginStartTimeKey] as Stopwatch;

			if (stopwatch == null) return;

			stopwatch.Stop();

			NR.Api.Agent.NewRelic.RecordResponseTimeMetric($"Custom/Renderings/{GetMetricName(args)}", stopwatch.ElapsedMilliseconds);
		}

		protected virtual string GetMetricName(RenderRenderingArgs args)
		{
			return args.Rendering.RenderingItem.DisplayName.Replace(" ", "_");
		}
	}
}