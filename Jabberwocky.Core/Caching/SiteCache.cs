using System;
using System.Runtime.Caching;
using System.Threading;
using Jabberwocky.Core.Caching.Base;

namespace Jabberwocky.Core.Caching
{
	public class SiteCache : BaseCacheProvider
	{
		private const string SiteCacheKey = "SiteCache";
		private static MemoryCache _internalCache = new MemoryCache(SiteCacheKey);

		private static readonly Lazy<SiteCache> SingletonCache = new Lazy<SiteCache>();

		/// <summary>
		/// Default singleton instance of the SiteCache
		/// </summary>
		/// <remarks>
		/// For use where IoC is not available
		/// </remarks>
		public static ICacheProvider Default => SingletonCache.Value;

		protected override ObjectCache Cache => _internalCache;

		/// <summary>
		/// Event handler to clear out the site cache
		/// </summary>
		/// <remarks>
		/// Internal cache is static, so it doesn't matter which instance of this class is used to clear the cache
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void EmptyCacheHandler(object sender, EventArgs args)
		{
			EmptyCache();
		}

		public override void EmptyCache()
		{
			var oldCache = Interlocked.Exchange(ref _internalCache, new MemoryCache(SiteCacheKey));
			oldCache.Dispose();
		}
	}
}
