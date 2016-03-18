using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Jabberwocky.Core.Caching;
using Jabberwocky.Core.Caching.Base;
using NUnit.Framework;

namespace Jabberwocky.Core.Tests.Caching
{
	[TestFixture]
	public abstract class CacheTestBase<T> where T : ICacheProvider
	{
		/// <summary>
		/// SUT
		/// </summary>
		protected T _cacheProvider;

		protected abstract ObjectCache InnerCache { get; }

		[SetUp]
		protected virtual void TestInit()
		{
			_cacheProvider = CreateTestProvider();
		}

		[TearDown]
		protected virtual void TestCleanup()
		{
			var disposable = InnerCache as IDisposable;

			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		protected abstract T CreateTestProvider();

		// SYNC

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public virtual void GetFromCache_NullKey_ReturnsNull()
		{
			_cacheProvider.GetFromCache<object>(null);
		}

		[Test]
		public virtual void GetFromCache_ValidKey_ItemExists_AreSame()
		{
			const string key = "key";
			var obj = new object();

			InnerCache.Add(key, obj, DateTime.Now + TimeSpan.FromDays(1));
			var retVal = _cacheProvider.GetFromCache<object>("key");
			Assert.AreSame(obj, retVal);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public virtual void GetFromCache_NullKey_NullCallback_Throws()
		{
			_cacheProvider.GetFromCache<object>(null, null);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public virtual void GetFromCache_EmptyString_NullCallback_Throws()
		{
			_cacheProvider.GetFromCache<object>(string.Empty, null);
		}

		[Test]
		public virtual void GetFromCache_Key_ReturnsValue()
		{
			var obj = new object();

			var returnVal = _cacheProvider.GetFromCache("key", () => obj);
			Assert.AreSame(obj, returnVal);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public virtual void AddToCache_NullKey_Throws()
		{
			_cacheProvider.AddToCache<string>(null, null);
		}

		[Test]
		public virtual void AddToCache_Key_NullObj_Successful()
		{
			const string key = "key";

			_cacheProvider.AddToCache<string>(key, null);
			Assert.IsFalse(InnerCache.Contains(key));
		}

		[Test]
		public virtual void AddToCache_Key_Obj_Successful()
		{
			const string key = "key";
			var obj = new object();
			_cacheProvider.AddToCache<object>(key, obj);
			Assert.IsTrue(InnerCache.Contains(key));
			Assert.AreSame(obj, InnerCache.Get(key));
		}

        [Test]
	    public virtual void GetFromCache_NullValue_Successful()
	    {
	        const string key = "key";
	        string retVal = null;
	        Assert.DoesNotThrow(() => retVal = _cacheProvider.GetFromCache<string>(key, () => null));
            Assert.IsNull(retVal);  

            // Validate that the null value was stored as a 'NullCacheEntry'
            Assert.IsNotNull(InnerCache.Get(key) as NullCacheEntry);
	    }

        // ASYNC

        [Test, ExpectedException(typeof(ArgumentNullException))]
		public virtual async Task GetFromCacheAsync_NullKey_ReturnsNull()
		{
			await _cacheProvider.GetFromCacheAsync<object>(null);
		}

		[Test]
		public virtual async Task GetFromCacheAsync_ValidKey_ItemExists_AreSame()
		{
			const string key = "key";
			var obj = new object();

			InnerCache.Add(key, obj, DateTime.Now + TimeSpan.FromDays(1));
			var retVal = await _cacheProvider.GetFromCacheAsync<object>("key");
			Assert.AreSame(obj, retVal);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public virtual async Task GetFromCacheAsync_NullKey_NullCallback_Throws()
		{
			await _cacheProvider.GetFromCacheAsync<object>(null, (Func<string>) null);
		}

		[Test, ExpectedException(typeof(ArgumentNullException))]
		public virtual async Task GetFromCacheAsync_EmptyString_NullCallback_Throws()
		{
			await _cacheProvider.GetFromCacheAsync<object>(string.Empty, (Func<string>)null);
		}

		[Test]
		public virtual async Task GetFromCacheAsync_Key_ReturnsValue()
		{
			var obj = new object();

			var returnVal = await _cacheProvider.GetFromCacheAsync("key", () => obj);
			Assert.AreSame(obj, returnVal);
		}

		[Test]
		public virtual async Task GetFromCacheAsync_Key_AsyncCallback_ReturnsValue()
		{
			var obj = new object();

			var returnVal = await _cacheProvider.GetFromCacheAsync("key", async ct => await Task.FromResult(obj));
			Assert.AreSame(obj, returnVal);
		}

		[Test]
		public virtual async Task AddToCacheAsync_Key_NullObj_Successful()
		{
			const string key = "key";

			await _cacheProvider.AddToCacheAsync<string>(key, null);
			Assert.IsFalse(InnerCache.Contains(key));
		}

		[Test]
		public virtual async Task AddToCacheAsync_Key_Obj_Successful()
		{
			const string key = "key";
			var obj = new object();
			await _cacheProvider.AddToCacheAsync(key, obj);
			Assert.IsTrue(InnerCache.Contains(key));
			Assert.AreSame(obj, InnerCache.Get(key));
		}

        [Test]
        public virtual async Task GetFromCacheAsync_NullValue_Successful()
        {
            const string key = "key";
            string retVal = await _cacheProvider.GetFromCacheAsync<string>(key, () => null);
            Assert.IsNull(retVal);

            // Validate that the null value was stored as a 'NullCacheEntry'
            Assert.IsNotNull(InnerCache.Get(key) as NullCacheEntry);
        }
    }
}
