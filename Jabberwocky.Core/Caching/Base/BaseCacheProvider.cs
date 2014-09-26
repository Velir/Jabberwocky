using System;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Jabberwocky.Core.Utils;

namespace Jabberwocky.Core.Caching.Base
{
	public abstract class BaseCacheProvider : ICacheProvider
	{
		protected abstract ObjectCache Cache { get; }

		public abstract void EmptyCache();

		/// <summary>
		/// Retrieves the desired object from the cache. If the object is null, executes the callback
		/// method to set it up and store it in the cache.
		/// </summary>
		/// <typeparam name="T">A reference type</typeparam>
		/// <param name="key">The cache key, must be unique for each object</param>
		/// <param name="callback">A callback method to retrieve the object if it is not in cache.</param>
		/// <returns></returns>
		public virtual T GetFromCache<T>(string key, Func<T> callback) where T : class
		{
			return GetFromCache<T>(key, TimeSpan.Zero, callback);
		}

		/// <summary>
		/// Retrieves the desired object from the cache. If the object is null, executes the callback
		/// method to set it up and store it in the cache.
		/// </summary>
		/// <typeparam name="T">A reference type</typeparam>
		/// <param name="key">The cache key, must be unique for each object</param>
		/// <param name="absoluteExpiration">A TimeSpan after which the item will expire from the cache.</param>
		/// <param name="callback">A callback method to retrieve the object if it is not in cache.</param>
		/// <returns></returns>
		public virtual T GetFromCache<T>(string key, TimeSpan absoluteExpiration, Func<T> callback) where T : class
		{
			if (key == null) throw new ArgumentNullException("key");
			if (callback == null) throw new ArgumentNullException("callback");

			// First Get is optimistic
			T cacheItem = Cache.Get(key) as T;
			if (cacheItem != null)
			{
				return cacheItem;
			}

			// Second Get tries again from Cache (just in case we were pre-empted by another thread's Add).
			// Failing that, it atomically adds to the cache and returns the computed value from the callback.
			// Note: the reason we're not using Cache.AddOrGetExisting() is because we care about potentially long-running callbacks... 
			//       we don't want to execute it multiple times when calling method.

			T value;
			// This lock is here to avoid edge-case where we have multiple threads executing a long-running callback
			// i.e. Only do it once, so first thread actually executes callback, re-entrant threads grab from cache.
			// Note: In very worst case, callback can still be called multiple times if the item is evicted too quickly.
			using (key.GetLock())
			{
				var item = Cache.Get(key) as T;
				value = item ?? callback();
				var expiry = absoluteExpiration == TimeSpan.Zero ? ObjectCache.InfiniteAbsoluteExpiration : DateTime.UtcNow.Add(absoluteExpiration);
				Cache.Add(key, value, expiry);
			}
			return value;
		}

		/// <summary>
		/// Adds an object to the cache; if it already exists, it will overwrite the existing item.
		/// </summary>
		/// <typeparam name="T">A reference type</typeparam>
		/// <param name="key">The cache key, must be unique for each object</param>
		/// <param name="value">The object to store in the cache</param>
		public virtual void AddToCache<T>(string key, T value) where T : class
		{
			if (key == null) throw new ArgumentNullException("key");

			if (value == null)
			{
				Cache.Remove(key);
			}
			else
			{
				Cache.Set(key, value, ObjectCache.InfiniteAbsoluteExpiration);
			}
		}

		/// <summary>
		/// Retrieves the desired object from the cache.  If it doesn't exist, returns null
		/// </summary>
		/// <typeparam name="T">A reference type</typeparam>
		/// <param name="key">The cache key, must be unique for each object</param>
		/// <returns>The requested object by key, or null if not found</returns>
		public virtual T GetFromCache<T>(string key) where T : class
		{
			if (key == null) throw new ArgumentNullException("key");

			return Cache.Get(key) as T;
		}

		#region IAsyncCacheProvider Implementation

		private static readonly Task CompletedTask = Task.FromResult(true);

		public virtual Task<T> GetFromCacheAsync<T>(string key, Func<T> callback, CancellationToken token = default(CancellationToken)) where T : class
		{
			return GetFromCacheAsync(key, TimeSpan.Zero, callback, token);
		}

		public virtual async Task<T> GetFromCacheAsync<T>(string key, TimeSpan absoluteExpiration, Func<T> callback, CancellationToken token = default(CancellationToken))
			where T : class
		{
			if (key == null) throw new ArgumentNullException("key");
			if (callback == null) throw new ArgumentNullException("callback");

			return await GetFromCacheAsync<T>(key, absoluteExpiration, ct => Task.FromResult(callback()), token);
		}

		public virtual Task<T> GetFromCacheAsync<T>(string key, Func<CancellationToken, Task<T>> callback, CancellationToken token = default(CancellationToken)) where T : class
		{
			return GetFromCacheAsync(key, TimeSpan.Zero, callback, token);
		}

		public virtual async Task<T> GetFromCacheAsync<T>(string key, TimeSpan absoluteExpiration, Func<CancellationToken, Task<T>> callback,
			CancellationToken token = new CancellationToken()) where T : class
		{
			if (key == null) throw new ArgumentNullException("key");
			if (callback == null) throw new ArgumentNullException("callback");

			// First Get is optimistic
			T cacheItem = Cache.Get(key) as T;
			if (cacheItem != null)
			{
				return cacheItem;
			}

			// Second Get tries again from Cache (just in case we were pre-empted by another thread's Add).
			// Failing that, it atomically adds to the cache and returns the computed value from the callback.
			// Note: the reason we're not using Cache.AddOrGetExisting() is because we care about potentially long-running callbacks... 
			//       we don't want to execute it multiple times when calling method.

			T value;
			// This lock is here to avoid edge-case where we have multiple threads executing a long-running callback
			// i.e. Only do it once, so first thread actually executes callback, re-entrant threads grab from cache.
			// Note: In very worst case, callback can still be called multiple times if the item is evicted too quickly.
			using (await key.GetLockAsync(token).ConfigureAwait(false))
			{
				var item = Cache.Get(key) as T;
				value = item ?? await callback(token).ConfigureAwait(false);
				var expiry = absoluteExpiration == TimeSpan.Zero ? ObjectCache.InfiniteAbsoluteExpiration : DateTime.UtcNow.Add(absoluteExpiration);
				Cache.Add(key, value, expiry);
			}
			return value;
		}

		public virtual Task AddToCacheAsync<T>(string key, T value, CancellationToken token = default(CancellationToken)) where T : class
		{
			if (key == null) throw new ArgumentNullException("key");

			if (value == null)
			{
				Cache.Remove(key);
			}
			else
			{
				Cache.Set(key, value, ObjectCache.InfiniteAbsoluteExpiration);
			}

			return CompletedTask;
		}

		public virtual Task<T> GetFromCacheAsync<T>(string key, CancellationToken token = default(CancellationToken)) where T : class
		{
			if (key == null) throw new ArgumentNullException("key");

			return Task.FromResult(Cache.Get(key) as T);
		}

		#endregion

	}
}
