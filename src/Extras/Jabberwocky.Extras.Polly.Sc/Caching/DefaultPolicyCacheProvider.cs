using System;
using System.Runtime.Caching;
using Jabberwocky.Core.Caching;
using Polly;

namespace Jabberwocky.Extras.Polly.Sc.Caching
{
	public class PolicyCacheProvider : GeneralCache, IPolicyCacheProvider
	{
		private static readonly Lazy<PolicyCacheProvider> LazyDefaultProvider = new Lazy<PolicyCacheProvider>();
		public static PolicyCacheProvider Default => LazyDefaultProvider.Value;

		public PolicyCacheProvider() : base(new MemoryCache("Polly Policy Cache"))
		{
		}

		public Policy GetOrAddPolicy(string policyKey, Func<string, Policy> policyFactory)
		{
			return GetFromCache(policyKey, () => policyFactory(policyKey));
		}

		public void ClearCache()
		{
			EmptyCache();
		}
	}
}
