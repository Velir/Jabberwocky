using Jabberwocky.Extras.Polly.Sc.Caching;
using Jabberwocky.Extras.Polly.Sc.Caching.Keys;
using Jabberwocky.Extras.Polly.Sc.Constants;
using Jabberwocky.Extras.Polly.Sc.Renderer;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Extras.Polly.Sc.Pipelines.Mvc.RenderRendering
{
	public class AddPollyWrapper : RenderRenderingProcessor
	{
		public override void Process(RenderRenderingArgs args)
		{
			if (args.Rendered) return;

			var rendering = args.Rendering;
			var renderer = rendering?.Renderer;
			if (renderer == null) return;

			// Check if the current rendering has enabled the circuit breaker; if not, don't do anything
			if (!IsCircuitBreakerEnabled(rendering)) return;

			// We're going to use Service Location here (in lieu of IoC) for OOTB Sitecore support
			// Also, let's request this service on every call, for maximum flexiblity (ie. for lifetime scope control, dynamic resolution)
			var policyKeyProvider = ServiceLocator.ServiceProvider.GetService<IPolicyKeyProvider>() ?? DefaultPolicyKeyProvider.Default;
			var policyCacheProvider = ServiceLocator.ServiceProvider.GetService<IPolicyCacheProvider>() ?? PolicyCacheProvider.Default;

			rendering.Renderer = new CircuitBreakerRendererDecorator(renderer, rendering.RenderingItem, policyCacheProvider, policyKeyProvider);
		}

		private bool IsCircuitBreakerEnabled(Rendering rendering)
		{
			var renderingItem = rendering.RenderingItem.InnerItem;

			return renderingItem[FieldConstants.UseCircuitBreaker].ToBool();
		}
	}
}
