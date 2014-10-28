using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Sites;

namespace Jabberwocky.Glass.Services
{
	public class SiteContextService : ISiteContextService
	{
		private SiteContext Site { get { return Sitecore.Context.Site; } }

		public string CurrentSiteName
		{
			get { return Sitecore.Context.GetSiteName() ?? string.Empty; }
		}

		public string StartPath
		{
			get { return Site != null ? Site.StartPath : string.Empty; }
		}

		public Language CurrentLanguage
		{
			get { return Sitecore.Context.Language; }
		}

		public DeviceItem CurrentDevice
		{
			get { return Sitecore.Context.Device; }
		}

		public string LanguageName
		{
			get { return Sitecore.Context.Language.Name; }
		}

	}
}
