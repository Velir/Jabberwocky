using System;
using System.Runtime.Caching;
using Jabberwocky.Core.Caching;
using NUnit.Framework;

namespace Jabberwocky.Core.Tests.Caching
{
	[TestFixture]
	public class GeneralCacheTests : CacheTestBase<GeneralCache>
	{
		private MemoryCache _innerCache;

		private static readonly DateTimeOffset TestExpiration = DateTime.Now + TimeSpan.FromDays(1);

		protected override ObjectCache InnerCache => _innerCache;

		protected override GeneralCache CreateTestProvider()
		{
            _innerCache = new MemoryCache("in-memory");
			return new GeneralCache(_innerCache);
		}

		[Test]
		public void EmptyCache_ClearsInnerCache()
		{
			InnerCache.Add("1", new object(), TestExpiration);
			InnerCache.Add("2", new object(), TestExpiration);

			Assert.AreEqual(2, InnerCache.GetCount());
			_cacheProvider.EmptyCache();

			Assert.AreEqual(0, InnerCache.GetCount());
			Assert.IsNull(_cacheProvider.GetFromCache<object>("1"));
			Assert.IsNull(_cacheProvider.GetFromCache<object>("2"));
		}
	}
}
