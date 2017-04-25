using System;
using System.IO;
using Jabberwocky.Extras.Polly.Sc.Caching;
using Jabberwocky.Extras.Polly.Sc.Caching.Keys;
using Jabberwocky.Extras.Polly.Sc.Constants;
using Polly;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Jabberwocky.Extras.Polly.Sc.Renderer
{
    public class CircuitBreakerRendererDecorator : BaseRendererDecorator
    {
        private readonly Sitecore.Mvc.Presentation.Renderer _innerRenderer;
        private readonly RenderingItem _renderingItem;
        private readonly IPolicyCacheProvider _cacheProvider;
        private readonly IPolicyKeyProvider _keyProvider;

        public CircuitBreakerRendererDecorator(Sitecore.Mvc.Presentation.Renderer innerRenderer,
            RenderingItem renderingItem,
            IPolicyCacheProvider cacheProvider,
            IPolicyKeyProvider keyProvider) : base(innerRenderer)
        {
            _innerRenderer = innerRenderer ?? throw new ArgumentNullException(nameof(innerRenderer));
            _renderingItem = renderingItem ?? throw new ArgumentNullException(nameof(renderingItem));
            _cacheProvider = cacheProvider ?? throw new ArgumentNullException(nameof(cacheProvider));
            _keyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));
        }

        public override void Render(TextWriter writer)
        {
            int numExceptions;
            int seconds;
            if (!int.TryParse(_renderingItem.InnerItem[FieldConstants.BreakAfterExceptionCount], out numExceptions)
                || !int.TryParse(_renderingItem.InnerItem[FieldConstants.OpenCircuitDurationInSeconds], out seconds))
            {
                // Could not parse the values to integers, so just pass through
                Log.Warn($"Configuration values on rendering '{_renderingItem.ID}' for " +
                         $"'{FieldConstants.BreakAfterExceptionCount}' or '{FieldConstants.OpenCircuitDurationInSeconds}' were invalid.", this);

                _innerRenderer.Render(writer);
                return;
            }

            var policyKey = _keyProvider.GetKey(_renderingItem);

            var policy = _cacheProvider.GetOrAddPolicy(policyKey,
                key => Policy.Handle<Exception>().CircuitBreaker(numExceptions, TimeSpan.FromSeconds(seconds)));

            policy.Execute(() => _innerRenderer.Render(writer));
        }
    }
}
