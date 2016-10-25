using System;
using System.IO;
using System.Web;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Jabberwocky.Extras.Polly.Sc.Renderer
{
	public class PageEditorRendererDecorator : Sitecore.Mvc.Presentation.Renderer
	{
		private readonly Sitecore.Mvc.Presentation.Renderer _innerRenderer;
		private readonly RenderingItem _renderingItem;

		public PageEditorRendererDecorator(Sitecore.Mvc.Presentation.Renderer innerRenderer, RenderingItem renderingItem)
		{
			if (innerRenderer == null) throw new ArgumentNullException(nameof(innerRenderer));
			if (renderingItem == null) throw new ArgumentNullException(nameof(renderingItem));
			_innerRenderer = innerRenderer;
			_renderingItem = renderingItem;
		}

		public override void Render(TextWriter writer)
		{
			// Only bother with capturing exceptions if we're in Experience Editor mode
			// No point in catching and immediately rethrowing when not in 'happy path'
			if (Context.PageMode.IsExperienceEditor)
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
			else
			{
				_innerRenderer.Render(writer);
			}
		}
	}
}
