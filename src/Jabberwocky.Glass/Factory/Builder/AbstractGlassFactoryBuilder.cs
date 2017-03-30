using System;
using Jabberwocky.Glass.Factory.Configuration;

namespace Jabberwocky.Glass.Factory.Builder
{
	public abstract class AbstractGlassFactoryBuilder : IGlassFactoryBuilder
	{
		protected IConfigurationOptions Options { get; }

		protected AbstractGlassFactoryBuilder(IConfigurationOptions options)
		{
			if (options == null) throw new ArgumentNullException(nameof(options));
			Options = options;
		}

		public abstract IGlassAdapterFactory BuildFactory();
	}
}
