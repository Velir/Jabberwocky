using System;
using System.IO;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Jabberwocky.Extras.Polly.Sc.Renderer
{
    public class ErrorHandlingRendererDecorator : BaseRendererDecorator
    {
        private readonly Sitecore.Mvc.Presentation.Renderer _innerRenderer;
        private readonly RenderingItem _renderingItem;

        public ErrorHandlingRendererDecorator(Sitecore.Mvc.Presentation.Renderer innerRenderer, RenderingItem renderingItem)
            : base(innerRenderer)
        {
            _innerRenderer = innerRenderer ?? throw new ArgumentNullException(nameof(innerRenderer));
            _renderingItem = renderingItem ?? throw new ArgumentNullException(nameof(renderingItem));
        }

        public override void Render(TextWriter writer)
        {
            try
            {
                _innerRenderer.Render(writer);
            }
            catch (Exception ex)
            {
                Log.Error($"Hiding rendering '{_renderingItem.ID}' due to error.", ex, typeof(ErrorHandlingRendererDecorator));
            }
        }
    }
}
