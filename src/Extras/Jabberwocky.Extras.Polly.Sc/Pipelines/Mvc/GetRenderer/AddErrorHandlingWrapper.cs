using Jabberwocky.Extras.Polly.Sc.Constants;
using Jabberwocky.Extras.Polly.Sc.Renderer;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Pipelines.Response.GetRenderer;
using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Extras.Polly.Sc.Pipelines.Mvc.GetRenderer
{
	public class AddErrorHandlingWrapper : GetRendererProcessor
	{
		public override void Process(GetRendererArgs args)
		{
			if (args.Result == null) return;

			var rendering = args.Rendering;
			var renderer = args.Result;
			if (rendering == null) return;

			// Check if the current rendering has enabled 'hiding on error'; if not, don't do anything
			if (!IsErrorHandlingEnabled(rendering)) return;

			args.Result = new ErrorHandlingRendererDecorator(renderer, rendering.RenderingItem);
		}

		private bool IsErrorHandlingEnabled(Rendering rendering)
		{
			return rendering.RenderingItem.InnerItem[FieldConstants.HideOnError].ToBool();
		}
	}
}
