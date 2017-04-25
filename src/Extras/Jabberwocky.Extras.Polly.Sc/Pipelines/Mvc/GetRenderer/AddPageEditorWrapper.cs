using Jabberwocky.Extras.Polly.Sc.Renderer;
using Sitecore;
using Sitecore.Mvc.Pipelines.Response.GetRenderer;

namespace Jabberwocky.Extras.Polly.Sc.Pipelines.Mvc.GetRenderer
{
    public class AddPageEditorWrapper : GetRendererProcessor
    {
        public override void Process(GetRendererArgs args)
        {
            if (args.Result == null) return;

            var renderer = args.Result;
            var rendering = args.Rendering;
            if (rendering == null) return;

            // Only apply this decorator when in Experience Editor
            if (!Context.PageMode.IsExperienceEditor) return;

            args.Result = new PageEditorRendererDecorator(renderer, rendering.RenderingItem);
        }
    }
}
