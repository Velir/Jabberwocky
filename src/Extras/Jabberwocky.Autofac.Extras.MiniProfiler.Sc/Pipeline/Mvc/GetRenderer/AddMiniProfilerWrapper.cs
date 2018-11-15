using Jabberwocky.Extras.MiniProfiler.Sc.Configuration;
using Jabberwocky.Extras.MiniProfiler.Sc.Renderer;
using Sitecore.Mvc.Pipelines.Response.GetRenderer;

namespace Jabberwocky.Extras.MiniProfiler.Sc.Pipeline.Mvc.GetRenderer
{
	public class AddMiniProfilerWrapper : GetRendererProcessor
	{
		public override void Process(GetRendererArgs args)
		{
			if (args.Result == null || !MiniProfilerConfiguration.IsMiniProfilerInitialized) return;

			var rendering = args.Rendering;
			var renderer = args.Result;

			if (renderer == null) return;

			args.Result = new MiniProfilerRenderer(renderer, rendering);
		}
	}
}
