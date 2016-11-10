using System;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;

namespace Jabberwocky.Extras.Polly.Sc.Caching
{
	public class CacheClearService
	{
		public void EmptyCacheHandler(object sender, EventArgs args)
		{
			var provider = ServiceLocator.ServiceProvider.GetService<IPolicyCacheProvider>() ?? PolicyCacheProvider.Default;

			provider.ClearCache();
		}
	}
}
