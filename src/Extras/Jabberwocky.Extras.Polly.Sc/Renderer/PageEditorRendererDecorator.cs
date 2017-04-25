using System;
using System.IO;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Jabberwocky.Extras.Polly.Sc.Renderer
{
    public class PageEditorRendererDecorator : BaseRendererDecorator
    {
        private readonly Sitecore.Mvc.Presentation.Renderer _innerRenderer;
        private readonly RenderingItem _renderingItem;

        public PageEditorRendererDecorator(Sitecore.Mvc.Presentation.Renderer innerRenderer, RenderingItem renderingItem)
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
                writer.Write("<p class='page-editor-component-error rendering-name'>Error rendering the following component: {0}</p><p class='page-editor-component-error exception-message'>{1}</p>",
                    HttpUtility.HtmlEncode(_renderingItem.Name),
                    HttpUtility.HtmlEncode(ex));
                Log.Error(ex.Message, ex, typeof(PageEditorRendererDecorator));
            }
        }
    }
}
