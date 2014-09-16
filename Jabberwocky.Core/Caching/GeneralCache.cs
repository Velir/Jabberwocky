using System;
using System.Runtime.Caching;
using System.Threading;
using Jabberwocky.Core.Caching.Base;

namespace Jabberwocky.Core.Caching
{
	public class GeneralCache : BaseCacheProvider
	{
		private MemoryCache _internalCache;

		public GeneralCache(MemoryCache internalCache)
		{
			if (internalCache == null) throw new ArgumentNullException("internalCache");
			_internalCache = internalCache;
		}

		protected override ObjectCache Cache
		{
			get { return _internalCache; }
		}

		public override void EmptyCache()
		{
			var oldCache = Interlocked.Exchange(ref _internalCache, new MemoryCache("internalCache"));
			oldCache.Dispose();
		}
	}
}
