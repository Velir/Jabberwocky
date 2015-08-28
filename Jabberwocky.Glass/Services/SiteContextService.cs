using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Sites;

namespace Jabberwocky.Glass.Services
{
	public class SiteContextService : ISiteContextService
	{
		private SiteContext Site => Sitecore.Context.Site;

		public string CurrentSiteName => Sitecore.Context.GetSiteName() ?? string.Empty;

		public string StartPath => Site != null ? Site.StartPath : string.Empty;

		public Language CurrentLanguage => Sitecore.Context.Language;

		public DeviceItem CurrentDevice => Sitecore.Context.Device;

		public string LanguageName => Sitecore.Context.Language.Name;
	}
}
