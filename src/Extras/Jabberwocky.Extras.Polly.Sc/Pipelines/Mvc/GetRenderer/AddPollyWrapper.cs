using Jabberwocky.Extras.Polly.Sc.Caching;
using Jabberwocky.Extras.Polly.Sc.Caching.Keys;
using Jabberwocky.Extras.Polly.Sc.Constants;
using Jabberwocky.Extras.Polly.Sc.Renderer;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Pipelines.Response.GetRenderer;
using Sitecore.Mvc.Presentation;

namespace Jabberwocky.Extras.Polly.Sc.Pipelines.Mvc.GetRenderer
{
	public class AddPollyWrapper : GetRendererProcessor
	{
		public override void Process(GetRendererArgs args)
		{
			if (args.Result == null) return;

			var rendering = args.Rendering;
			var renderer = args.Result;
			if (rendering == null) return;

			// Check if the current rendering has enabled the circuit breaker; if not, don't do anything
			if (!IsCircuitBreakerEnabled(rendering)) return;

			// We're going to use Service Location here (in lieu of IoC) for OOTB Sitecore support
			// Also, let's request this service on every call, for maximum flexiblity (ie. for lifetime scope control, dynamic resolution)
			var policyKeyProvider = ServiceLocator.ServiceProvider.GetService<IPolicyKeyProvider>() ?? DefaultPolicyKeyProvider.Default;
			var policyCacheProvider = ServiceLocator.ServiceProvider.GetService<IPolicyCacheProvider>() ?? PolicyCacheProvider.Default;

			args.Result = new CircuitBreakerRendererDecorator(renderer, rendering.RenderingItem, policyCacheProvider, policyKeyProvider);
		}

		private bool IsCircuitBreakerEnabled(Rendering rendering)
		{
			var renderingItem = rendering.RenderingItem.InnerItem;

			return renderingItem[FieldConstants.UseCircuitBreaker].ToBool();
		}
	}
}
