using System;
using System.IO;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Jabberwocky.Extras.Polly.Sc.Renderer
{
	public class ErrorHandlingRendererDecorator : Sitecore.Mvc.Presentation.Renderer
	{
		private readonly Sitecore.Mvc.Presentation.Renderer _innerRenderer;
		private readonly RenderingItem _renderingItem;

		public ErrorHandlingRendererDecorator(Sitecore.Mvc.Presentation.Renderer innerRenderer, RenderingItem renderingItem)
		{
			if (innerRenderer == null) throw new ArgumentNullException(nameof(innerRenderer));
			if (renderingItem == null) throw new ArgumentNullException(nameof(renderingItem));
			_innerRenderer = innerRenderer;
			_renderingItem = renderingItem;
		}

		public override void Render(TextWriter writer)
		{
			try
			{
				_innerRenderer.Render(writer);
			}
			catch (Exception ex)
			{
				Log.Warn($"Hiding rendering '{_renderingItem.ID}' due to error.", ex, typeof(ErrorHandlingRendererDecorator));
			}
		}
	}
}
