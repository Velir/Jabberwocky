using System;

namespace Jabberwocky.Extras.Polly.Sc.Renderer
{
    public abstract class BaseRendererDecorator : Sitecore.Mvc.Presentation.Renderer
    {
        private readonly Sitecore.Mvc.Presentation.Renderer _innerRenderer;

        protected BaseRendererDecorator(Sitecore.Mvc.Presentation.Renderer innerRenderer)
        {
            _innerRenderer = innerRenderer ?? throw new ArgumentNullException(nameof(innerRenderer));
        }

        public override string CacheKey => _innerRenderer.CacheKey;
    }
}
