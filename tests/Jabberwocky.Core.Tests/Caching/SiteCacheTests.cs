using System;
using System.Runtime.Caching;
using Jabberwocky.Core.Caching;
using Jabberwocky.Core.Testing;
using NUnit.Framework;

namespace Jabberwocky.Core.Tests.Caching
{
	[TestFixture]
	public class SiteCacheTests : CacheTestBase<SiteCache>
	{
		private static readonly DateTimeOffset TestExpiration = DateTime.Now + TimeSpan.FromDays(1);

		protected override ObjectCache InnerCache => ((dynamic)DynamicWrapper.For(_cacheProvider))._internalCache;

		protected override SiteCache CreateTestProvider()
		{
			return new SiteCache();
		}

		protected override void TestCleanup()
		{
			base.TestCleanup();

			// need to cleanup static data
			((dynamic)DynamicWrapper.For(_cacheProvider))._internalCache = new MemoryCache("in-memory");
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

		[Test]
		public void EmptyCacheHandler_Invoked_EmptiesCache()
		{
			InnerCache.Add("1", new object(), TestExpiration);
			InnerCache.Add("2", new object(), TestExpiration);

			Assert.AreEqual(2, InnerCache.GetCount());

			_cacheProvider.EmptyCacheHandler(new object(), new EventArgs());

			Assert.AreEqual(0, InnerCache.GetCount());
			Assert.IsNull(_cacheProvider.GetFromCache<object>("1"));
			Assert.IsNull(_cacheProvider.GetFromCache<object>("2"));
		}

		[Test]
		public void Default_SingletonInstance_UsesSameInnerCache()
		{
			var singleton = SiteCache.Default;

			var singletonCache = (DynamicWrapper.For((object)singleton))._internalCache;

			Assert.AreNotSame(_cacheProvider, singleton); // Are NOT the same SiteCache instance
			Assert.AreSame(InnerCache, singletonCache); // But ARE the same MemoryCache instance
		}
	}
}
