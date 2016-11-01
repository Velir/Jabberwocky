using Jabberwocky.Autofac.Extras.MiniProfiler.Sc.Renderer;
using Jabberwocky.Autofac.Extras.MiniProfiler.Util;
using Sitecore.Mvc.Pipelines.Response.GetRenderer;

namespace Jabberwocky.Autofac.Extras.MiniProfiler.Sc.Pipeline.Mvc.GetRenderer
{
	public class AddMiniProfilerWrapper : GetRendererProcessor
	{
		public override void Process(GetRendererArgs args)
		{
			if (args.Result == null || !MiniProfilerRuntime.MiniProfilerInitialized) return;

			var rendering = args.Rendering;
			var renderer = args.Result;

			if (renderer == null) return;

			args.Result = new MiniProfilerRenderer(renderer, rendering);
		}
	}
}
