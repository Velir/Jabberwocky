using System;
using System.Threading;
using System.Threading.Tasks;
using Glass.Mapper.Sc;
using Jabberwocky.Core.Caching;
using Jabberwocky.Glass.Services;

namespace Jabberwocky.Glass.Caching
{
	public class SitecoreCacheDecorator : ICacheProvider
	{
		protected readonly ICacheProvider Provider;
		protected readonly string DatabaseName;
		protected readonly string SiteName;
		protected readonly string LanguageName;

		/// <summary>
		/// Whether or not we should actually cache results, or should just pass through the cache.
		/// The cache is marked as 'pass-through' if the current database is 'master' (as we always want up-to-date 'master' items)
		/// </summary>
		protected readonly bool ShouldPassThrough;

		private const string IgnoreDatabaseName = "master";

		public SitecoreCacheDecorator(ICacheProvider provider, ISitecoreService database, ISiteContextService siteService)
		{
			if (provider == null)
			{
				throw new ArgumentNullException(nameof(provider), "A valid ICacheProvider instance must be provided.");
			}
			if (database == null)
			{
				throw new ArgumentNullException(nameof(database), "A valid ISitecoreService instance must be provided.");
			}
			if (siteService == null)
			{
				throw new ArgumentNullException(nameof(siteService));
			}
			Provider = provider;
			DatabaseName = database.Database.Name;
			SiteName = siteService.CurrentSiteName;
			LanguageName = siteService.LanguageName;

			ShouldPassThrough = DatabaseName.ToLowerInvariant() == IgnoreDatabaseName;
		}

		public void EmptyCache()
		{
			Provider.EmptyCache();
		}

		public T GetFromCache<T>(string key, Func<T> callback) where T : class
		{
			if (ShouldPassThrough)
			{
				return callback();
			}
			var newKey = GenerateCacheKey(key);
			return Provider.GetFromCache(newKey, callback);
		}

		public T GetFromCache<T>(string key, TimeSpan absoluteExpiration, Func<T> callback) where T : class
		{
			if (ShouldPassThrough)
			{
				return callback();
			}
			var newKey = GenerateCacheKey(key);
			return Provider.GetFromCache(newKey, absoluteExpiration, callback);
		}

		public void AddToCache<T>(string key, T value) where T : class
		{
			var newKey = GenerateCacheKey(key);
			Provider.AddToCache(newKey, value);
		}

		public T GetFromCache<T>(string key) where T : class
		{
			var newKey = GenerateCacheKey(key);
			return Provider.GetFromCache<T>(newKey);
		}

		protected virtual string GenerateCacheKey(string key)
		{
			var newKey = string.Format("SitecoreCacheDecorator:{0}-{1}-{2}-{3}", SiteName, DatabaseName, LanguageName, key);
			return newKey;
		}

		#region IAsyncCacheProvider Decorated Implementation

		public async Task<T> GetFromCacheAsync<T>(string key, Func<T> callback,
			CancellationToken token = new CancellationToken()) where T : class
		{
			if (ShouldPassThrough)
			{
				return callback();
			}
			var newKey = GenerateCacheKey(key);
			return await Provider.GetFromCacheAsync(newKey, callback, token).ConfigureAwait(false);
		}

		public async Task<T> GetFromCacheAsync<T>(string key, TimeSpan absoluteExpiration, Func<T> callback,
			CancellationToken token = new CancellationToken()) where T : class
		{
			if (ShouldPassThrough)
			{
				return callback();
			}
			var newKey = GenerateCacheKey(key);
			return await Provider.GetFromCacheAsync(newKey, absoluteExpiration, callback, token).ConfigureAwait(false);
		}

		public async Task<T> GetFromCacheAsync<T>(string key, Func<CancellationToken, Task<T>> callback,
			CancellationToken token = new CancellationToken()) where T : class
		{
			if (ShouldPassThrough)
			{
				return await callback(token).ConfigureAwait(false);
			}
			var newKey = GenerateCacheKey(key);
			return await Provider.GetFromCacheAsync(newKey, callback, token).ConfigureAwait(false);
		}

		public async Task<T> GetFromCacheAsync<T>(string key, TimeSpan absoluteExpiration, Func<CancellationToken, Task<T>> callback,
			CancellationToken token = new CancellationToken()) where T : class
		{
			if (ShouldPassThrough)
			{
				return await callback(token).ConfigureAwait(false);
			}
			var newKey = GenerateCacheKey(key);
			return await Provider.GetFromCacheAsync(newKey, absoluteExpiration, callback, token).ConfigureAwait(false);
		}

		public async Task AddToCacheAsync<T>(string key, T value, CancellationToken token = new CancellationToken()) where T : class
		{
			var newKey = GenerateCacheKey(key);
			await Provider.AddToCacheAsync(newKey, value, token).ConfigureAwait(false);
		}

		public async Task<T> GetFromCacheAsync<T>(string key, CancellationToken token = new CancellationToken()) where T : class
		{
			var newKey = GenerateCacheKey(key);
			return await Provider.GetFromCacheAsync<T>(newKey, token).ConfigureAwait(false);
		}

		#endregion

	}
}
