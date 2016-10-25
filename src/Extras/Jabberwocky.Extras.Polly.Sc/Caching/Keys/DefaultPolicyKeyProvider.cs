using System;
using Jabberwocky.Extras.Polly.Sc.Constants;
using Sitecore.Data.Items;
using Sitecore.Mvc.Extensions;

namespace Jabberwocky.Extras.Polly.Sc.Caching.Keys
{
	public class DefaultPolicyKeyProvider : IPolicyKeyProvider
	{
		private static readonly Lazy<DefaultPolicyKeyProvider> LazyKeyProvider = new Lazy<DefaultPolicyKeyProvider>();
		public static DefaultPolicyKeyProvider Default => LazyKeyProvider.Value;

		public string GetKey(RenderingItem renderingItem)
		{
			var variesByDatasource = renderingItem.InnerItem[FieldConstants.PolicyVariesByDatasource].ToBool();
			var variesByLanguage = renderingItem.InnerItem[FieldConstants.PolicyVariesByLanguage].ToBool();
			var variesBySite = renderingItem.InnerItem[FieldConstants.PolicyVariesBySite].ToBool();
			var datasourcePartialKey = variesByDatasource ? $":ds-{renderingItem.DataSource}" : string.Empty;
			var languagePartialKey = variesByLanguage ? $":lang-{Sitecore.Context.Language?.CultureInfo.Name}" : string.Empty;
			var sitePartialKey = variesBySite ? $":site-{Sitecore.Context.Site?.Name}" : string.Empty;

			// A composite key of Datasource, Language, & Site
			return $"{renderingItem.ID}{datasourcePartialKey}{languagePartialKey}{sitePartialKey}";
		}
	}
}
