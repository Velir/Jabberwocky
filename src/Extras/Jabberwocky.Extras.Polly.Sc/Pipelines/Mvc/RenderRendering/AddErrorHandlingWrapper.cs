using Jabberwocky.Extras.Polly.Sc.Constants;
using Jabberwocky.Extras.Polly.Sc.Renderer;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Extras.Polly.Sc.Pipelines.Mvc.RenderRendering
{
	public class AddErrorHandlingWrapper : RenderRenderingProcessor
	{
		public override void Process(RenderRenderingArgs args)
		{
			if (args.Rendered) return;

			var rendering = args.Rendering;
			var renderer = rendering?.Renderer;
			if (renderer == null) return;

			// Check if the current rendering has enabled 'hiding on error'; if not, don't do anything
			if (!IsErrorHandlingEnabled(rendering)) return;

			rendering.Renderer = new ErrorHandlingRendererDecorator(renderer, rendering.RenderingItem);
		}

		private bool IsErrorHandlingEnabled(Rendering rendering)
		{
			return rendering.RenderingItem.InnerItem[FieldConstants.HideOnError].ToBool();
		}
	}
}
