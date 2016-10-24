using Sitecore.Data.Items;

namespace Jabberwocky.Extras.Polly.Sc.Caching.Keys
{
	public interface IPolicyKeyProvider
	{
		string GetKey(RenderingItem renderingItem);
	}
}
