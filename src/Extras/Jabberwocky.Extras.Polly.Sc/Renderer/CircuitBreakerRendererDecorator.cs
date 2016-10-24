using System;
using System.IO;
using Jabberwocky.Extras.Polly.Sc.Caching;
using Jabberwocky.Extras.Polly.Sc.Caching.Keys;
using Jabberwocky.Extras.Polly.Sc.Constants;
using Polly;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Extensions;

namespace Jabberwocky.Extras.Polly.Sc.Renderer
{
	public class CircuitBreakerRendererDecorator : Sitecore.Mvc.Presentation.Renderer
	{
		private readonly Sitecore.Mvc.Presentation.Renderer _innerRenderer;
		private readonly RenderingItem _renderingItem;
		private readonly IPolicyCacheProvider _cacheProvider;
		private readonly IPolicyKeyProvider _keyProvider;

		public CircuitBreakerRendererDecorator(Sitecore.Mvc.Presentation.Renderer innerRenderer, 
			RenderingItem renderingItem, 
			IPolicyCacheProvider cacheProvider,
			IPolicyKeyProvider keyProvider)
		{
			if (innerRenderer == null) throw new ArgumentNullException(nameof(innerRenderer));
			if (renderingItem == null) throw new ArgumentNullException(nameof(renderingItem));
			if (cacheProvider == null) throw new ArgumentNullException(nameof(cacheProvider));
			if (keyProvider == null) throw new ArgumentNullException(nameof(keyProvider));
			_innerRenderer = innerRenderer;
			_renderingItem = renderingItem;
			_cacheProvider = cacheProvider;
			_keyProvider = keyProvider;
		}

		public override void Render(TextWriter writer)
		{
			int numExceptions;
			int.TryParse(_renderingItem.InnerItem[FieldConstants.BreakAfterExceptionCount], out numExceptions);
			int seconds;
			int.TryParse(_renderingItem.InnerItem[FieldConstants.OpenCircuitDurationInSeconds], out seconds);

			var policyKey = _keyProvider.GetKey(_renderingItem);

			var policy = _cacheProvider.GetOrAddPolicy(policyKey,
				key => Policy.Handle<Exception>().CircuitBreaker(numExceptions, TimeSpan.FromSeconds(seconds)));

			// Let's only 'try' if we can handle the exception
			if (ShouldHideRendering())
			{
				try
				{
					policy.Execute(() => _innerRenderer.Render(writer));
				}
				catch (Exception ex)
				{
					Log.Warn($"Hiding rendering '{_renderingItem.ID}' due to error.", ex, typeof(CircuitBreakerRendererDecorator));
				}
			}
			else
			{
				policy.Execute(() => _innerRenderer.Render(writer));
			}
		}

		private bool ShouldHideRendering()
		{
			return _renderingItem.InnerItem[FieldConstants.HideOnError].ToBool();
		}
	}
}
