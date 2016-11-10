namespace Jabberwocky.Core.Caching
{
	public interface ICacheProvider : ISyncCacheProvider, IAsyncCacheProvider
	{
		void EmptyCache();
	}
}
