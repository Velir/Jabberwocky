using Sitecore.Data.Items;
using Sitecore.Pipelines.HttpRequest;
using NR = NewRelic; 

namespace Jabberwocky.Extras.NewRelic.Sc.Pipelines.HttpRequestBegin
{
	public class TransactionNameProcessor : HttpRequestProcessor
	{
		public override void Process(HttpRequestArgs args)
		{
			Item currentItem = Sitecore.Context.Item;
			if (!string.IsNullOrEmpty(currentItem?.TemplateName))
			{
				NR.Api.Agent.NewRelic.SetTransactionName("Webpage", currentItem.TemplateName);
			}
		}
	}
}