using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Jabberwocky.Glass.Autofac.Pipelines.Factories.Providers;

namespace Jabberwocky.Glass.Autofac.Pipelines.Factories
{
    internal class DefaultLifetimeScopeFactory : ILifetimeScopeFactory
    {
        private readonly IEnumerable<ILifetimeScopeProvider> _scopeProviders;

        internal DefaultLifetimeScopeFactory(IEnumerable<ILifetimeScopeProvider> scopeProviders)
        {
            if (scopeProviders == null) throw new ArgumentNullException(nameof(scopeProviders));
            _scopeProviders = scopeProviders;
        }

        public ILifetimeScope GetCurrentLifetimeScope()
        {
            return _scopeProviders
                .Select(provider => provider.GetLifetimeScope())
                .FirstOrDefault(scope => scope != null);
        }
    }
}
