using System;
using Polly;

namespace Jabberwocky.Extras.Polly.Sc.Caching
{
	public interface IPolicyCacheProvider
	{
		Policy GetOrAddPolicy(string policyKey, Func<string, Policy> policyFactory);

		void ClearCache();
	}
}
