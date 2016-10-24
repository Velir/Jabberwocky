using Jabberwocky.Extras.Polly.Sc.Renderer;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;

namespace Jabberwocky.Extras.Polly.Sc.Pipelines.Mvc.RenderRendering
{
	public class AddPageEditorWrapper : RenderRenderingProcessor
	{
		public override void Process(RenderRenderingArgs args)
		{
			if (args.Rendered) return;

			var rendering = args.Rendering;

			rendering.Renderer = new PageEditorRendererDecorator(rendering.Renderer, rendering.RenderingItem);
		}
	}
}
